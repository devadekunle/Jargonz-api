using jargonz.api.Common.Extensions;
using jargonz.api.Modules.Auth.Login;
using jargonz.api.Modules.Auth.MagicLink;
using jargonz.api.Modules.Auth.Register;
using jargonz.api.Modules.Auth.TokenRefresh;

namespace jargonz.api.Modules.Auth;

/// <summary>
///     Authentication module - groups all auth-related endpoints
/// </summary>
public class AuthModule : IModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        // Register all auth endpoints
        group.MapLoginEndpoint();
        group.MapRegisterEndpoint();
        group.MapGetMagicLinkEndpoint();
        group.MapTokenRefreshEndpoint();
    }
}
