using jargonz.api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace jargonz.api.Common.Persistence.EntityConfigurations;

public class WordOfTheDayCacheConfig : IEntityTypeConfiguration<WordOfTheDayCache>
{
    public void Configure(EntityTypeBuilder<WordOfTheDayCache> builder)
    {
        builder.ToTable("WordOfTheDayCache");

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.CachedAt)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.Date }).IsUnique();
    }
}
