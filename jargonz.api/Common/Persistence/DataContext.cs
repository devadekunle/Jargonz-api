using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using jargonz.api.Domain;

namespace jargonz.api.Common.Persistence;

public class DataContext(DbContextOptions<DataContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Ulid>, Ulid>(options)
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<MagicLinkToken> MagicLinkTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Ulid>()
            .HaveConversion<UlidToStringConverter>();
    }
}

public class UlidToStringConverter : ValueConverter<Ulid, string>
{
    private static readonly ConverterMappingHints DefaultHints = new(26);

    public UlidToStringConverter() : this(null)
    {
    }

    public UlidToStringConverter(ConverterMappingHints? mappingHints)
        : base(
            x => x.ToString(),
            x => Ulid.Parse(x),
            DefaultHints.With(mappingHints))
    {
    }
}
