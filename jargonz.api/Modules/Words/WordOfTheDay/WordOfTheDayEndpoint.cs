using jargonz.api.Common.Extensions;
using jargonz.api.Common.Results;
using Wolverine;

namespace jargonz.api.Modules.Words.WordOfTheDay;

public static class WordOfTheDayEndpoint
{
    public static IEndpointRouteBuilder MapWordOfTheDayEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/words/wotd", async (IMessageBus bus, HttpContext httpContext) =>
            {
                var userId = httpContext.GetUserId();
                var query = new WordOfTheDayQuery(userId);
                var result = await bus.InvokeAsync<Result<WordOfTheDayResponse>>(query);
                return result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithName("WordOfTheDay")
            .WithTags("Words")
            .WithSummary("Get the Word of the Day")
            .WithDescription("Returns a deterministic word of the day for the authenticated user. The same word is shown all day, and changes daily.")
            .Produces<WordOfTheDayResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
