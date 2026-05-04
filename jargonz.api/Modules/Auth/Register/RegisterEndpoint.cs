using jargonz.api.Common.Results;
using Wolverine;

namespace jargonz.api.Modules.Auth.Register;

/// <summary>
///     Endpoint configuration for user registration
/// </summary>
public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (RegisterCommand command, IMessageBus bus) =>
            {
                var result = await bus.InvokeAsync<Result<RegisterResponse>>(command);
                return result.IsSuccess
                    ? result.ToCreatedHttpResult($"/api/users/{result.Value.UserId}")
                    : result.ToHttpResult();
            })
            .WithName("Register")
            .WithTags("Authentication")
            .WithSummary("Register new user")
            .WithDescription("Creates a new user account.")
            .Produces<RegisterResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);

        return app;
    }
}
