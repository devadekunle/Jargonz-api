using jargonz.api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace jargonz.api.Common.Persistence.EntityConfigurations;

public class WordEntryConfig : IEntityTypeConfiguration<WordEntry>
{
    public void Configure(EntityTypeBuilder<WordEntry> builder)
    {
        builder.ToTable("Words");

        builder.Property(x => x.Word)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.WordHash)
            .HasMaxLength(64)
            .IsRequired();


        builder.Property(x => x.Definition)
            .HasMaxLength(2000);

        builder.Property(x => x.Phonetic)
            .HasMaxLength(100);

        builder.Property(x => x.PartOfSpeech)
            .HasMaxLength(50);

        builder.Property(x => x.Etymology)
            .HasMaxLength(1000);

        builder.Property(x => x.ContextSentence)
            .HasMaxLength(2000);

        builder.Property(x => x.ExampleSentence)
            .HasMaxLength(2000);

        builder.Property(x => x.UserNotes)
            .HasMaxLength(2000);

        builder.Property(x => x.PageNumber);

        builder.Property(x => x.EaseFactor)
            .IsRequired();

        builder.Property(x => x.Interval)
            .IsRequired();

        builder.Property(x => x.Repetitions)
            .IsRequired();

        builder.Property(x => x.NextReviewDate)
            .IsRequired();

        builder.Property(x => x.TimesReviewed)
            .IsRequired();

        builder.Property(x => x.TimesCorrect)
            .IsRequired();


        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Book)
            .WithMany()
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.NextReviewDate);
        builder.HasIndex(x => x.Word);
        builder.HasIndex(x => x.WordHash).IsUnique();
    }
}
