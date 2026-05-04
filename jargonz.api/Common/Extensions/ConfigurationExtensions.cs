using jargonz.api.Common.Configuration;
using jargonz.api.Common.EmailNotifications;
using jargonz.api.Common.Llm;

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

        serviceCollection.AddDeepSeekServices(configuration);

        serviceCollection.AddEmailNotifications(configuration);
    }
}
