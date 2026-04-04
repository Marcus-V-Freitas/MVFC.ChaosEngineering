namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that overwrites the Content-Type header with an invalid or unexpected value.
/// </summary>
internal sealed class ContentTypeCorruptionHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.ContentTypeCorruption;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.ContentType = decision.CorruptContentType;
            return Task.CompletedTask;
        });

        await next(context).ConfigureAwait(false);
    }
}
