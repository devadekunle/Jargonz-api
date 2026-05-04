using jargonz.api.Common.Results;

namespace jargonz.api.Modules.Auth;

/// <summary>
///     Authentication and authorization related errors
/// </summary>
public static class AuthErrors
{
    public static Error InvalidCredentials => Error.Create(
        "Auth.InvalidCredentials",
        "The provided credentials are invalid");

    public static Error EmailAlreadyExists => Error.Conflict(
        "EmailAlreadyExists",
        "A user with this email already exists");

    public static Error UserNotFound => Error.NotFound("User", "specified");

    public static Error TokenExpired => Error.Unauthorized(
        "The authentication token has expired");

    public static Error InvalidToken => Error.Unauthorized(
        "The authentication token is invalid");

    public static Error RegistrationFailed => Error.Validation(
        "RegistrationFailed",
        "Registration failed");
}
