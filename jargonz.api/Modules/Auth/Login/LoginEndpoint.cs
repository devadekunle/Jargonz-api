using jargonz.api.Common.Results;
using Wolverine;

namespace jargonz.api.Modules.Auth.Login;

/// <summary>
///     Endpoint configuration for login
/// </summary>
public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (LoginCommand command, IMessageBus bus) =>
            {
                var result = await bus.InvokeAsync<Result<LoginResponse>>(command);
                return result.ToHttpResult();
            })
            .WithName("Login")
            .WithTags("Authentication")
            .WithSummary("User login")
            .WithDescription("Validate magic link token and returns a JWT token")
            .Produces<LoginResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
