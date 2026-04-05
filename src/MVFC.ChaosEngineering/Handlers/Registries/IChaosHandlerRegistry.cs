namespace MVFC.ChaosEngineering.Handlers.Registries;

/// <summary>
/// Registry for all available chaos handlers, providing a central point for handler resolution.
/// </summary>
internal interface IChaosHandlerRegistry
{
    /// <summary>
    /// Gets the handler for the specified chaos kind.
    /// </summary>
    /// <param name="kind">The chaos kind.</param>
    /// <returns>The corresponding <see cref="IChaosHandler"/>.</returns>
    public IChaosHandler GetHandler(ChaosKind kind);
}
