using Z.EntityFramework.Plus;

namespace jargonz.api.Common.Query;

public static class QueryExtensions
{
    public static async Task<PaginatedResponse<T>> ApplyPaginationAsync<T>(this IQueryable<T> query,
        QueryModel queryModel,
        CancellationToken cancellationToken)
    {
        var totalCount = query.DeferredCount().FutureValue();
        var page = query.Skip((queryModel.PageNumber.Value - 1) * queryModel.PageSize.Value)
            .Take(queryModel.PageSize.Value)
            .Future();
        return query.ToPaginatedList(await page.ToListAsync(cancellationToken), await totalCount.ValueAsync(cancellationToken), queryModel.PageNumber.Value,
            queryModel.PageSize.Value);
    }

    public static PaginatedResponse<T> ToPaginatedList<T>(
        this IQueryable<T> query,
        List<T> page,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        return new PaginatedResponse<T>(page, totalCount, pageNumber, pageSize, totalPages);
    }
}
