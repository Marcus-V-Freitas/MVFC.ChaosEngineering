namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that returns a specific HTTP status code.
/// </summary>
internal sealed class StatusCodeHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.StatusCode;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        context.Response.StatusCode = decision.StatusCode;
        await context.Response.WriteAsync($"Chaos engineering: HTTP {decision.StatusCode} injected.", context.RequestAborted).ConfigureAwait(false);
    }
}
