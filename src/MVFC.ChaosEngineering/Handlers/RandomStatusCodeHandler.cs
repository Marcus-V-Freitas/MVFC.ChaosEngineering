namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that returns a random 5xx error status code.
/// </summary>
internal sealed class RandomStatusCodeHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.RandomStatusCode;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosRule rule,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        var candidates = new[] { 500, 502, 503, 504 };
        var status = candidates[Random.Shared.Next(candidates.Length)];
        context.Response.StatusCode = status;
        await context.Response.WriteAsync($"Chaos engineering: random HTTP {status} injected.", context.RequestAborted).ConfigureAwait(false);
    }
}
