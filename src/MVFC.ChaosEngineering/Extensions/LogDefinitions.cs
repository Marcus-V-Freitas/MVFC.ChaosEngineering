namespace MVFC.ChaosEngineering.Extensions;

public static partial class LogDefinitions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Chaos Engineering: Injecting {ChaosKind} fault at {Path}. ChaosId: {ChaosId}")]
    public static partial void LogChaosInjected(this ILogger logger, string chaosKind, string path, string chaosId);
}
