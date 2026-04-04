namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that injects custom HTTP headers into the response.
/// </summary>
internal sealed class HeaderInjectionHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.HeaderInjection;

    /// <inheritdoc />
    public async Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        if (decision.Headers is { Count: > 0 })
        {
            context.Response.OnStarting(() =>
            {
                foreach (var (name, value) in decision.Headers)
                    context.Response.Headers[name] = value;
                return Task.CompletedTask;
            });
        }

        await next(context).ConfigureAwait(false);
    }
}
