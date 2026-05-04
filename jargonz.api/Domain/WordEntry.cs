namespace jargonz.api.Domain;

public class WordEntry
{
    public Ulid Id { get; init; } = Ulid.NewUlid();
    public required string Word { get; init; }
    public string WordHash { get; set; } = string.Empty;
    public Ulid BookId { get; set; }
    public Book Book { get; set; }
    public string Definition { get; set; } = string.Empty;
    public string Phonetic { get; set; } = string.Empty;
    public string PartOfSpeech { get; set; } = string.Empty;
    public string Etymology { get; set; } = string.Empty;
    public string ContextSentence { get; set; } = string.Empty;
    public string ExampleSentence { get; set; } = string.Empty;
    public string UserNotes { get; set; } = string.Empty;
    public int? PageNumber { get; set; }
    public double EaseFactor { get; set; } = 2.5;
    public int Interval { get; set; }
    public int Repetitions { get; set; }
    public DateOnly NextReviewDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public int TimesReviewed { get; set; }
    public int TimesCorrect { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public AppUser User { get; set; }
    public Ulid UserId { get; set; }
}
