using jargonz.api.Common.Extensions;
using jargonz.api.Common.Results;
using Wolverine;

namespace jargonz.api.Modules.Books.GetBooks;

public static class GetBooksEndpoint
{
    public static IEndpointRouteBuilder MapGetBooksEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/books", async (IMessageBus bus, HttpContext httpContext) =>
            {
                var query = new GetBooksQuery(httpContext.GetUserId());
                var result = await bus.InvokeAsync<Result<IReadOnlyList<GetBooksResponse>>>(query);
                return result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithName("GetBooks")
            .WithTags("Books")
            .WithSummary("List all books for the current user")
            .WithDescription("Returns all books belonging to the authenticated user, ordered by most recent.")
            .Produces<IReadOnlyList<GetBooksResponse>>()
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
