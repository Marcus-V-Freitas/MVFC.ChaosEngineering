namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that simulates a request timeout by delaying indefinitely until the request is aborted.
/// </summary>
internal sealed class TimeoutHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.Timeout;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path) =>
            await Task.Delay(Timeout.InfiniteTimeSpan, context.RequestAborted).ConfigureAwait(false);
}
