namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that simulates a request timeout by delaying longer than a typical client timeout.
/// </summary>
internal sealed class TimeoutHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.Timeout;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosRule rule,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path) => // Induce a delay that exceeds typical client timeouts (e.g. 100 seconds)        
            await Task.Delay(TimeSpan.FromSeconds(100), context.RequestAborted).ConfigureAwait(false);
}
