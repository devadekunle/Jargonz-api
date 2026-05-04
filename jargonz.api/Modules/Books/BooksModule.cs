using jargonz.api.Common.Extensions;
using jargonz.api.Modules.Books.CreateBook;
using jargonz.api.Modules.Books.GetBooks;

namespace jargonz.api.Modules.Books;

/// <summary>
///     Books module - groups all book-related endpoints
/// </summary>
public class BooksModule : IModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1")
            .WithTags("Books");

        group.MapCreateBookEndpoint();
        group.MapGetBooksEndpoint();
    }
}
