namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that sends only a partial response body and then aborts the connection.
/// </summary>
internal sealed class PartialResponseHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.PartialResponse;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/plain";

        var partial = Encoding.UTF8.GetBytes(new string('x', decision.PartialBytes));
        await context.Response.Body.WriteAsync(partial, context.RequestAborted).ConfigureAwait(false);
        await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);

        context.Abort();
    }
}
