using jargonz.api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace jargonz.api.Common.Persistence.EntityConfigurations;

public class BookConfig : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Author)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.CoverColor)
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion<string>()
            .HasMaxLength(26)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.Title);
    }
}
