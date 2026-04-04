namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that returns a 200 OK but with an empty response body.
/// </summary>
internal sealed class EmptyBodyHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.EmptyBody;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosRule rule,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentLength = 0;
        await Task.CompletedTask;
    }
}
