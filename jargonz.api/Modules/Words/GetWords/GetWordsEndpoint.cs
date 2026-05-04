using jargonz.api.Common.Extensions;
using jargonz.api.Common.Query;
using jargonz.api.Common.Results;
using Wolverine;

namespace jargonz.api.Modules.Words.GetWords;

public static class GetWordsEndpoint
{
    public static IEndpointRouteBuilder MapGetWordsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/words", async (IMessageBus bus, HttpContext httpContext, [AsParameters] GetWordsQuery query) =>
            {
                query.UserId = httpContext.GetUserId();
                var result = await bus.InvokeAsync<Result<PaginatedResponse<GetWordResponse>>>(query);
                return result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithName("GetWords")
            .WithTags("Words")
            .WithSummary("List all words for the current user")
            .WithDescription("Returns paginated words belonging to the authenticated user. Supports filtering by book, mastery level, due for review, and full-text search.")
            .Produces<PaginatedResponse<GetWordResponse>>()
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
