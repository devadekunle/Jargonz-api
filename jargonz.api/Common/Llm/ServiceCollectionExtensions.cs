using jargonz.api.Common.Configuration;

namespace jargonz.api.Common.Llm;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDeepSeekServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var deepSeekSettings = configuration.GetSection("DeepSeek").Get<DeepSeekSettings>();
        deepSeekSettings?.Validate();

        services.AddSingleton(deepSeekSettings!);

        services.AddHttpClient<DeepSeekApiClient>(client =>
        {
            client.BaseAddress = new Uri(deepSeekSettings!.BaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {deepSeekSettings.ApiKey}");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<WordEnrichmentService>();

        return services;
    }
}
