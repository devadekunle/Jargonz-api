using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using jargonz.api.Domain;

namespace jargonz.api.Common.Persistence.EntityConfigurations;

public class MagicLinkTokenConfig : IEntityTypeConfiguration<MagicLinkToken>
{
    public void Configure(EntityTypeBuilder<MagicLinkToken> builder)
    {
        builder.HasKey(x => new { x.Email, x.Token });

        builder.Property(x => x.Token)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();
    }
}
