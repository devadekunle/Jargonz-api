using Microsoft.AspNetCore.Mvc;

namespace jargonz.api.Common.Query;

public record QueryModel
{
    [FromQuery(Name = "search")] public string? SearchKeyword { get; set; }

    [FromQuery(Name = "page")] public int? PageNumber { get; set; } = 1;

    [FromQuery(Name = "size")] public int? PageSize { get; set; } = 25;
}

public record PaginatedResponse<T>(List<T> Data, int TotalCount, int PageNumber, int PageSize, int TotalPages)
{
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}
