namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that aborts the current TCP connection immediately.
/// </summary>
internal sealed class AbortHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.Abort;

    /// <inheritdoc />
    public Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        context.Abort();
        return Task.CompletedTask;
    }
}
