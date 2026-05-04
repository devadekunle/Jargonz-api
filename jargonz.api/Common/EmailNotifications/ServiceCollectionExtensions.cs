using jargonz.api.Common.Configuration;
using Resend;

namespace jargonz.api.Common.EmailNotifications;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailNotifications(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection("Email").Get<EmailSettings>();
        emailSettings?.Validate();

        services.AddSingleton(emailSettings!);

        services.AddHttpClient();
        services.Configure<ResendClientOptions>(options => { options.ApiToken = emailSettings?.ApiKey ?? ""; });
        services.AddScoped<IResend, ResendClient>();

        services
            .AddFluentEmail(emailSettings?.FromEmail ?? "noreply@example.com", emailSettings?.FromName ?? "Jargonz")
            .AddRazorRenderer();

        services.AddScoped<EmailNotificationService>();

        return services;
    }
}
