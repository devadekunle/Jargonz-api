namespace jargonz.api.Domain;

public class Book
{
    public Ulid Id { get; init; } = Ulid.NewUlid();
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string CoverColor { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public AppUser User { get; set; }
    public Ulid UserId { get; set; }
}
