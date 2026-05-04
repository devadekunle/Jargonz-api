namespace jargonz.api.Common.Extensions;

/// <summary>
///     Extension methods for organizing endpoint registration
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    ///     Maps all module endpoints to the application
    /// </summary>
    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder app)
    {
        // Automatically discover and register all modules
        var moduleType = typeof(IModule);
        var modules = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => moduleType.IsAssignableFrom(p) && p is { IsClass: true, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IModule>();

        foreach (var module in modules) module.MapEndpoints(app);

        return app;
    }
}

/// <summary>
///     Interface for feature modules to implement
/// </summary>
public interface IModule
{
    void MapEndpoints(IEndpointRouteBuilder app);
}
