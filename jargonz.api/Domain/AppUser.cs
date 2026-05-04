using Microsoft.AspNetCore.Identity;

namespace jargonz.api.Domain;

public class AppUser : IdentityUser<Ulid>
{
    public required string FullName { get; init; }
}
