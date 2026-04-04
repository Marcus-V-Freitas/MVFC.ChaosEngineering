namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that limits the response body output to a fixed number of bytes per second.
/// </summary>
internal sealed class BandwidthThrottleHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.BandwidthThrottle;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        var originalBody = context.Response.Body;
        using var capture = new MemoryStream();
        context.Response.Body = capture;

        try
        {
            await next(context).ConfigureAwait(false);
        }
        finally
        {
            context.Response.Body = originalBody;
        }

        const int CHUNK_SIZE = 8;
        var delay = TimeSpan.FromSeconds((double)CHUNK_SIZE / Math.Max(1, decision.BytesPerSecond));

        await ThrottledWriter.WriteAsync(
            originalBody,
            capture.ToArray(),
            CHUNK_SIZE,
            delay,
            context.RequestAborted).ConfigureAwait(false);
    }
}
