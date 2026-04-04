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
        ChaosRule rule,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        context.Response.StatusCode = rule.RedirectStatusCode;
        context.Response.Headers.Location = rule.RedirectUrl ?? "/";
        return Task.CompletedTask;
    }
}
