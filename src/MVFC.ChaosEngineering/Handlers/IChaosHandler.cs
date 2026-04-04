namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Defines a handler for a specific kind of chaos fault.
/// </summary>
internal interface IChaosHandler
{
    /// <summary>
    /// Gets the kind of chaos this handler supports.
    /// </summary>
    public ChaosKind Kind { get; }

    /// <summary>
    /// Executes the chaos injection logic.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="decision">The chaos decision.</param>
    /// <param name="next">The next request delegate in the pipeline.</param>
    /// <param name="instrumentation">The instrumentation service.</param>
    /// <param name="path">The request path for metrics/logging.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path);
}
