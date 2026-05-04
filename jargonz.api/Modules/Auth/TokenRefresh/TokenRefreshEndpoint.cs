using jargonz.api.Common.Results;
using Wolverine;

namespace jargonz.api.Modules.Auth.TokenRefresh;

/// <summary>
///     Endpoint configuration for token refresh
/// </summary>
public static class TokenRefreshEndpoint
{
    public static IEndpointRouteBuilder MapTokenRefreshEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/refresh", async (TokenRefreshCommand command, IMessageBus bus) =>
            {
                var result = await bus.InvokeAsync<Result<TokenRefreshResponse>>(command);
                return result.ToHttpResult();
            })
            .WithName("RefreshToken")
            .WithTags("Authentication")
            .WithSummary("Refresh access token")
            .WithDescription("Refresh an expired access token using a valid refresh token")
            .Produces<TokenRefreshResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
