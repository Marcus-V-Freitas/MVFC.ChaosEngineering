namespace MVFC.ChaosEngineering.Exceptions;

/// <summary>Default exception thrown by <see cref="ChaosKind.Exception"/> when no specific type is configured.</summary>
public sealed class ChaosException : Exception
{
    public ChaosException()
        : base("Chaos engineering: intentional exception injected.") { }
}
