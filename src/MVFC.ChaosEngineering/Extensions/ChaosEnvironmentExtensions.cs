namespace MVFC.ChaosEngineering.Extensions;

/// <summary>Helper extensions for the <see cref="ChaosEnvironment"/> enum.</summary>
internal static class ChaosEnvironmentExtensions
{
    /// <summary>Checks if a <see cref="ChaosEnvironment"/> value matches the environment string.</summary>
    /// <param name="env">The enum value.</param>
    /// <param name="aspNetCoreEnvironment">The environment string (e.g. "Development").</param>
    /// <returns><c>true</c> if it matches; otherwise, <c>false</c>.</returns>
    internal static bool Matches(this ChaosEnvironment env, string aspNetCoreEnvironment)
        => string.Equals(env.ToString(), aspNetCoreEnvironment, StringComparison.OrdinalIgnoreCase);
}
