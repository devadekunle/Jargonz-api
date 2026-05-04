namespace jargonz.api.Domain;

public class MagicLinkToken
{
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(1);
    public DateTime? UsedAt { get; set; }
}
