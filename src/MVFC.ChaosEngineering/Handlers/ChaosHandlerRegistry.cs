namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Registry for all available chaos handlers, providing a central point for handler resolution.
/// </summary>
internal static class ChaosHandlerRegistry
{
    private static readonly Dictionary<ChaosKind, IChaosHandler> _handlers =
        new List<IChaosHandler>
        {
            new AbortHandler(),
            new BandwidthThrottleHandler(),
            new ContentTypeCorruptionHandler(),
            new CorruptBodyHandler(),
            new EmptyBodyHandler(),
            new ExceptionHandler(),
            new ForcedRedirectHandler(),
            new HeaderInjectionHandler(),
            new LatencyHandler(),
            new PartialResponseHandler(),
            new RandomLatencyHandler(),
            new RandomStatusCodeHandler(),
            new SlowBodyHandler(),
            new StatusCodeHandler(),
            new ThrottleHandler(),
            new TimeoutHandler()
        }.ToDictionary(h => h.Kind);

    private static readonly IChaosHandler _defaultHandler = new ExceptionHandler();

    /// <summary>
    /// Gets the handler for the specified chaos kind.
    /// </summary>
    /// <param name="kind">The chaos kind.</param>
    /// <returns>The corresponding <see cref="IChaosHandler"/>, or a default handler if not found.</returns>
    public static IChaosHandler GetHandler(ChaosKind kind)
    {
        return _handlers.TryGetValue(kind, out var handler)
            ? handler
            : _defaultHandler;
    }
}
