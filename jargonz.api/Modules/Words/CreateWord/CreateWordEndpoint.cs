using jargonz.api.Common.Extensions;
using jargonz.api.Common.Results;
using Wolverine;

namespace jargonz.api.Modules.Words.CreateWord;

public static class CreateWordEndpoint
{
    public static IEndpointRouteBuilder MapCreateWordEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/words", async (CreateWordCommand command, IMessageBus bus, HttpContext httpContext) =>
            {
                var cmd = command with { UserId = httpContext.GetUserId() };
                var result = await bus.InvokeAsync<Result<CreateWordResponse>>(cmd);
                return result.IsSuccess
                    ? result.ToCreatedHttpResult($"/api/v1/words/{result.Value.Id}")
                    : result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithName("CreateWord")
            .WithTags("Words")
            .WithSummary("Create a new word entry")
            .WithDescription("Adds a new word to a book. The word's definition, phonetic, part of speech, etymology, and example sentence will be populated by an LLM.")
            .Produces<CreateWordResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
