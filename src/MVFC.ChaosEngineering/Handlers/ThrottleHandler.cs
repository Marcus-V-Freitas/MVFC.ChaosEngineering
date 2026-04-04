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
        ChaosRule rule,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        var retryAfterSeconds = rule.RetryAfter.HasValue
            ? (int)Math.Ceiling(rule.RetryAfter.Value.TotalSeconds)
            : 5;

        context.Response.StatusCode = rule.StatusCode;
        context.Response.Headers.RetryAfter = retryAfterSeconds.ToString(CultureInfo.InvariantCulture);
        await context.Response.WriteAsync($"Chaos engineering: HTTP {rule.StatusCode} too many requests injected.", context.RequestAborted).ConfigureAwait(false);
    }
}
