namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that injects a random delay within a specified range before letting the request proceed.
/// </summary>
internal sealed class RandomLatencyHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.RandomLatency;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosRule rule,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        var minMs = (int)rule.MinLatency.TotalMilliseconds;
        var maxMs = (int)rule.MaxLatency.TotalMilliseconds;
        var delayMs = Random.Shared.Next(minMs, maxMs + 1);

        instrumentation.RecordLatency(delayMs, "RandomLatency", path);

        await Task.Delay(TimeSpan.FromMilliseconds(delayMs), context.RequestAborted).ConfigureAwait(false);
        await next(context).ConfigureAwait(false);
    }
}
