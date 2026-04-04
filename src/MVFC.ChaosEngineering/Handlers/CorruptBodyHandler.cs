namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that returns a 200 OK but with a corrupted or incomplete response body.
/// </summary>
internal sealed class CorruptBodyHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.CorruptBody;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        var body = decision.CorruptedBody ?? "{\"chaos\":true,\"truncated\":";
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(body, context.RequestAborted).ConfigureAwait(false);
    }
}
