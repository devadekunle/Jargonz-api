using jargonz.api.Common.Configuration;
using jargonz.api.Common.EmailNotifications;

namespace jargonz.api.Common.Extensions;

public static class ConfigurationExtensions
{
    public static void ConfigureSettings(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var allowedDomainSettings = configuration.GetSection("AllowedDomain").Get<AllowedDomainSettings>();
        allowedDomainSettings?.Validate();
        serviceCollection.AddSingleton(allowedDomainSettings!);

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
        jwtSettings?.Validate();
        serviceCollection.AddSingleton(jwtSettings!);

        serviceCollection.AddEmailNotifications(configuration);
    }
}
