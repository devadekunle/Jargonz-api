using System.Security.Claims;

namespace jargonz.api.Common.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    ///     Gets the authenticated user's ID from the JWT claims.
    /// </summary>
    public static Ulid GetUserId(this HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Ulid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User ID not found in claims");

        return userId;
    }
}
