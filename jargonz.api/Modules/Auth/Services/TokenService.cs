using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using jargonz.api.Common.Configuration;
using jargonz.api.Common.Persistence;
using jargonz.api.Domain;

namespace jargonz.api.Modules.Auth.Services;

public static class TokenService
{
    public static async Task<string> GenerateAccessTokenAsync(
        JwtSettings jwtSettings,
        AppUser user,
        UserManager<AppUser> userManager)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

        // Build claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Audience = jwtSettings.Audience,
            Issuer = jwtSettings.Issuer
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);
        return accessToken;
    }

    public static RefreshToken BuildRefreshToken(AppUser user)
    {
        return new RefreshToken { UserId = user.Id };
    }

    public static async Task<RefreshToken> CreateAndSaveRefreshTokenAsync(
        AppUser user,
        DataContext context,
        CancellationToken cancellationToken)
    {
        var refreshToken = BuildRefreshToken(user);
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(cancellationToken);
        return refreshToken;
    }
}
