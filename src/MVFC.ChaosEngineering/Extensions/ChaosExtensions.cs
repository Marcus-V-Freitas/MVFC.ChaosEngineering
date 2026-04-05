namespace MVFC.ChaosEngineering.Extensions;

/// <summary>
/// Extension methods for integrating <see cref="ChaosMiddleware"/> into the ASP.NET Core pipeline.
/// </summary>
public static class ChaosExtensions
{
    /// <summary>
    /// Registers chaos engineering services and configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration delegate.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddChaos(this IServiceCollection services, Action<ChaosPolicyBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.TryAddSingleton<ChaosInstrumentation>();
        services.AddSingleton<IChaosHandlerRegistry, ChaosHandlerRegistry>();
        services.AddSingleton<IOptionsFactory<ChaosPolicy>>(sp => new ChaosPolicyFactory(configure));

        return services;
    }

    /// <summary>
    /// Adds <see cref="ChaosMiddleware"/> to the pipeline using the supplied policy.
    /// </summary>
    public static IApplicationBuilder UseChaos(this IApplicationBuilder app, ChaosPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(policy);

        var logger = app.ApplicationServices.GetRequiredService<ILogger<ChaosMiddleware>>();
        var instrumentation = app.ApplicationServices.GetRequiredService<ChaosInstrumentation>();
        var handlerRegistry = app.ApplicationServices.GetRequiredService<IChaosHandlerRegistry>();

        return app.UseMiddleware<ChaosMiddleware>(logger, instrumentation, handlerRegistry, policy);
    }

    /// <summary>
    /// Adds <see cref="ChaosMiddleware"/> to the pipeline using a fluent builder.
    /// </summary>
    public static IApplicationBuilder UseChaos(this IApplicationBuilder app, Action<ChaosPolicyBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new ChaosPolicyBuilder();
        configure(builder);
        return app.UseChaos(builder.Build());
    }

    /// <summary>
    /// Adds <see cref="ChaosMiddleware"/> to the pipeline, resolving the policy from the DI container (Options pattern).
    /// </summary>
    public static IApplicationBuilder UseChaos(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var logger = app.ApplicationServices.GetRequiredService<ILogger<ChaosMiddleware>>();
        var instrumentation = app.ApplicationServices.GetRequiredService<ChaosInstrumentation>();
        var handlerRegistry = app.ApplicationServices.GetRequiredService<IChaosHandlerRegistry>();

        return app.UseMiddleware<ChaosMiddleware>(logger, instrumentation, handlerRegistry);
    }
}
