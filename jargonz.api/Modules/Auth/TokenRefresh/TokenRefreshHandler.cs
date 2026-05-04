using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using jargonz.api.Common.Configuration;
using jargonz.api.Common.Persistence;
using jargonz.api.Common.Results;
using jargonz.api.Domain;
using jargonz.api.Modules.Auth.Services;

namespace jargonz.api.Modules.Auth.TokenRefresh;

public static class TokenRefreshHandler
{
    public static async Task<Result<TokenRefreshResponse>> HandleAsync(TokenRefreshCommand command,
        DataContext context,
        JwtSettings jwtSettings,
        UserManager<AppUser> userManager,
        CancellationToken cancellationToken)
    {
        var id = Ulid.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(command.RefreshToken)));
        var existingToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Id == id, cancellationToken);

        if (existingToken == null)
            return Result.Failure<TokenRefreshResponse>(AuthErrors.InvalidToken);

        if (existingToken.ExpiresAt < DateTime.UtcNow)
            return Result.Failure<TokenRefreshResponse>(AuthErrors.TokenExpired);

        var user = await userManager.FindByIdAsync(existingToken.UserId.ToString());
        if (user == null)
            return Result.Failure<TokenRefreshResponse>(AuthErrors.UserNotFound);

        var accessToken = await TokenService.GenerateAccessTokenAsync(jwtSettings, user, userManager);
        var newRefreshToken = await TokenService.CreateAndSaveRefreshTokenAsync(user, context, cancellationToken);

        return Result.Success(new TokenRefreshResponse(accessToken, jwtSettings.Expiration, user.Id, user.Email!,
            newRefreshToken.Token));
    }
}
