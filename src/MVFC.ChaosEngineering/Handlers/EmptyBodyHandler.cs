namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that returns a 200 OK with no content and Content-Length set to 0.
/// </summary>
internal sealed class EmptyBodyHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.EmptyBody;

    /// <inheritdoc />
    public Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentLength = 0;
        return Task.CompletedTask;
    }
}
