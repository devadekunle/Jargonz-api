using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using jargonz.api.Common.Configuration;
using jargonz.api.Common.Persistence;
using jargonz.api.Common.Results;
using jargonz.api.Domain;
using jargonz.api.Modules.Auth.Services;

namespace jargonz.api.Modules.Auth.Login;

public static class LoginHandler
{
    public static async Task<Result<LoginResponse>> HandleAsync(LoginCommand command,
        DataContext context,
        JwtSettings jwtSettings,
        UserManager<AppUser> userManager,
        CancellationToken cancellationToken)
    {
        var pendingToken = await context.MagicLinkTokens.FirstOrDefaultAsync(x =>
            x.Email == command.Email
            && x.Token == command.MagicToken
            && x.UsedAt == null, cancellationToken);

        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null || pendingToken == null)
            return Result.Failure<LoginResponse>(Error.Unauthorized());

        pendingToken.UsedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        var accessToken = await TokenService.GenerateAccessTokenAsync(jwtSettings, user, userManager);
        var refreshToken = await TokenService.CreateAndSaveRefreshTokenAsync(user, context, cancellationToken);

        return Result.Success(new LoginResponse(accessToken, jwtSettings.Expiration, user.Id, user.Email!,
            refreshToken.Token));
    }
}
