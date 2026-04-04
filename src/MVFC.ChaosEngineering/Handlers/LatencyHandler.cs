namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that injects a fixed delay before letting the request proceed.
/// </summary>
internal sealed class LatencyHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.Latency;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosRule rule,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        var ms = rule.Latency.TotalMilliseconds;
        instrumentation.RecordLatency(ms, "Latency", path);

        await Task.Delay(rule.Latency, context.RequestAborted).ConfigureAwait(false);
        await next(context).ConfigureAwait(false);
    }
}
