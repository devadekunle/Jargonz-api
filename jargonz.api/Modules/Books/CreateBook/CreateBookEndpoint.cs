using jargonz.api.Common.Extensions;
using jargonz.api.Common.Results;
using Wolverine;

namespace jargonz.api.Modules.Books.CreateBook;

public static class CreateBookEndpoint
{
    public static IEndpointRouteBuilder MapCreateBookEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/books", async (CreateBookCommand command, IMessageBus bus, HttpContext httpContext) =>
            {
                var cmd = command with { UserId = httpContext.GetUserId() };
                var result = await bus.InvokeAsync<Result<CreateBookResponse>>(cmd);
                return result.IsSuccess
                    ? result.ToCreatedHttpResult($"/api/books/{result.Value.Id}")
                    : result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithName("CreateBook")
            .WithTags("Books")
            .WithSummary("Create a new book")
            .WithDescription("Creates a new book with a randomly assigned cover color.")
            .Produces<CreateBookResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
