namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that forces a redirection by setting a 301/302 status code and the Location header.
/// </summary>
internal sealed class ForcedRedirectHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.ForcedRedirect;

    /// <inheritdoc />
    public Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        context.Response.StatusCode = decision.RedirectStatusCode;
        context.Response.Headers.Location = decision.RedirectUrl ?? "/";
        return Task.CompletedTask;
    }
}
