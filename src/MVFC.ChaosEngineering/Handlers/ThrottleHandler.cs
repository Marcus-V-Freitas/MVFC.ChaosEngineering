namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that returns a 429 Too Many Requests status code with a Retry-After header.
/// </summary>
internal sealed class ThrottleHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.Throttle;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        var retryAfterSeconds = decision.RetryAfter.HasValue
            ? (int)Math.Ceiling(decision.RetryAfter.Value.TotalSeconds)
            : 5;

        context.Response.StatusCode = decision.StatusCode;
        context.Response.Headers.RetryAfter = retryAfterSeconds.ToString(CultureInfo.InvariantCulture);
        await context.Response.WriteAsync($"Chaos engineering: HTTP {decision.StatusCode} too many requests injected.", context.RequestAborted).ConfigureAwait(false);
    }
}
