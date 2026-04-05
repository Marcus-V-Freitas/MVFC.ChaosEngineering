namespace MVFC.ChaosEngineering.Middleware;

/// <summary>
/// ASP.NET Core middleware that evaluates a <see cref="ChaosPolicy"/> on every request
/// and injects the configured chaos.
/// </summary>
internal sealed class ChaosMiddleware(
    RequestDelegate next,
    ILogger<ChaosMiddleware> logger,
    ChaosInstrumentation instrumentation,
    IChaosHandlerRegistry registry,
    ChaosPolicy? policy = null,
    IOptionsMonitor<ChaosPolicy>? options = null)
{
    private readonly RequestDelegate _next = next;
    private readonly IOptionsMonitor<ChaosPolicy>? _options = options;
    private readonly ILogger<ChaosMiddleware> _logger = logger;
    private readonly ChaosInstrumentation _instrumentation = instrumentation;

    private ChaosPolicy CurrentPolicy { get => field ?? _options?.CurrentValue ?? ChaosPolicy.Disabled; } = policy;

    /// <summary>Invokes the middleware for the given HTTP context.</summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        await InvokeAsyncCore(context).ConfigureAwait(false);
    }

    /// <summary>Core logic for evaluating policy and injecting chaos.</summary>
    private async Task InvokeAsyncCore(HttpContext context)
    {
        _instrumentation.RecordEvaluation();

        var rule = CurrentPolicy.Evaluate(context);

        if (rule is null)
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        var chaosId = Guid.NewGuid().ToString("N");
        var kindString = rule.Kind.ToString();
        var path = context.Request.Path.Value ?? "/";

        // Logging & Metrics
        _logger.LogChaosInjected(kindString, path, chaosId);
        _instrumentation.RecordFault(kindString, path);

        // OpenTelemetry Enrichment
        var activity = Activity.Current;
        if (activity is not null)
        {
            activity.SetTag("chaos.injected", true);
            activity.SetTag("chaos.kind", kindString);
            activity.SetTag("chaos.id", chaosId);
            activity.SetTag("chaos.path", path);
        }

        // Standard Headers
        context.Response.Headers["X-Chaos-Injected"] = "true";
        context.Response.Headers["X-Chaos-Id"] = chaosId;

        await InjectFaultAsync(context, rule, path).ConfigureAwait(false);
    }

    /// <summary>Injects the specific fault described by the <see cref="ChaosRule"/>.</summary>
    private async Task InjectFaultAsync(HttpContext context, ChaosRule rule, string path)
    {
        var handler = registry.GetHandler(rule.Kind);
        await handler.HandleAsync(context, rule, _next, _instrumentation, path).ConfigureAwait(false);
    }
}
