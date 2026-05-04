
using jargonz.api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace jargonz.api.Common.Persistence.EntityConfigurations;

public class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(x => x.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => x.FullName);
    }
}
