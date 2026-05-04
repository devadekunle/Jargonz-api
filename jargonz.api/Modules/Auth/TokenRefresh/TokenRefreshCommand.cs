using FluentValidation;

namespace jargonz.api.Modules.Auth.TokenRefresh;

/// <summary>
///     Command for token refresh
/// </summary>
public record TokenRefreshCommand(string RefreshToken);

/// <summary>
///     Response for successful token refresh
/// </summary>
public record TokenRefreshResponse(
    string AccessToken,
    TimeSpan ValidTill,
    Ulid UserId,
    string Email,
    string RefreshToken);

public class TokenRefreshCommandValidator : AbstractValidator<TokenRefreshCommand>
{
    public TokenRefreshCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}
