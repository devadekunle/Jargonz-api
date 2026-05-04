namespace jargonz.api.Domain;

public class WordOfTheDayCache
{
    public Ulid Id { get; init; } = Ulid.NewUlid();
    public Ulid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public DateOnly Date { get; set; }
    public Ulid WordEntryId { get; set; }
    public WordEntry WordEntry { get; set; } = null!;
    public DateTime CachedAt { get; init; } = DateTime.UtcNow;
}
