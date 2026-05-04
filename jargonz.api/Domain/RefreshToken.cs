using System.Text;

namespace jargonz.api.Domain;

public class RefreshToken
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddHours(1);
    public required Ulid UserId { get; init; }
    public string Token => Convert.ToBase64String(Encoding.UTF8.GetBytes(Id.ToString()));
}
