namespace jargonz.api.Modules.Auth.Login;

/// <summary>
///     Command for user login
/// </summary>
public record LoginCommand(string Email, string MagicToken);

/// <summary>
///     Response for successful login
/// </summary>
public record LoginResponse(string AccessToken, TimeSpan ValidTill, Ulid UserId, string Email, string RefreshToken);
