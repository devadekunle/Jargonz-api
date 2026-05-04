using jargonz.api.Common.Results;
using Wolverine;

namespace jargonz.api.Modules.Auth.MagicLink;

/// <summary>
///     Endpoint configuration for getting a magic link
/// </summary>
public static class GetMagicLinkEndpoint
{
    public static IEndpointRouteBuilder MapGetMagicLinkEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/magic-link", async (GetMagicLinkQuery query, IMessageBus bus) =>
            {
                var result = await bus.InvokeAsync<Result>(query);
                return result.ToHttpResult();
            })
            .WithName("GetMagicLink")
            .WithTags("Authentication")
            .WithSummary("Request a magic link")
            .WithDescription("Generates and stores a magic link token for the specified email address")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
