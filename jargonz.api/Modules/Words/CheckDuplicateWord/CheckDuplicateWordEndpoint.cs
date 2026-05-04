using Microsoft.EntityFrameworkCore;
using jargonz.api.Common.Extensions;
using jargonz.api.Common.Persistence;

namespace jargonz.api.Modules.Words.CheckDuplicateWord;

public static class CheckDuplicateWordEndpoint
{
    public static IEndpointRouteBuilder MapCheckDuplicateWordEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/words/check-duplicate", async (
                string word,
                string? contextSentence,
                HttpContext httpContext,
                DataContext context) =>
            {
                var userId = httpContext.GetUserId();

                var wordHash = word.ToWordHash(userId, contextSentence!);

                var exists = await context.WordEntries
                    .AnyAsync(w => w.WordHash == wordHash);

                return Results.Ok(new CheckDuplicateWordResponse(exists));
            })
            .RequireAuthorization()
            .WithName("CheckDuplicateWord")
            .WithTags("Words")
            .WithSummary("Check if a word already exists for the current user")
            .WithDescription("Checks if a word with the given text already exists for the current user. Returns true if a duplicate is found.")
            .Produces<CheckDuplicateWordResponse>()
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}

public record CheckDuplicateWordResponse(bool IsDuplicate);
