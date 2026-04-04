namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that simulates a slow response body by injecting delays between chunks of data.
/// </summary>
internal sealed class SlowBodyHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.SlowBody;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosRule rule,
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

        await ThrottledWriter.WriteAsync(
            originalBody,
            capture.ToArray(),
            rule.ChunkSize,
            rule.ChunkDelay,
            context.RequestAborted).ConfigureAwait(false);
    }
}
