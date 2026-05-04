using Microsoft.EntityFrameworkCore;
using jargonz.api.Common.Persistence;
using jargonz.api.Common.Results;

namespace jargonz.api.Modules.Books.GetBooks;

public static class GetBooksHandler
{
    public static async Task<Result<IReadOnlyList<GetBooksResponse>>> HandleAsync(
        GetBooksQuery query,
        DataContext context,
        CancellationToken ct)
    {
        var books = await context.Books
            .Where(b => b.UserId == query.UserId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new GetBooksResponse(
                b.Id,
                b.Title,
                b.Author,
                b.CoverColor,
                b.CreatedAt))
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<GetBooksResponse>>(books);
    }
}
