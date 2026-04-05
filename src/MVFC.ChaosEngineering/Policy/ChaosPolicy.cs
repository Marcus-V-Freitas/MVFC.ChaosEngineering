namespace MVFC.ChaosEngineering.Policy;

/// <summary>
/// Represents a collection of chaos rules that are evaluated against incoming requests.
/// </summary>
/// <remarks>
/// Rules are evaluated in order of specificity (longest pattern first).
/// The first rule that matches the request criteria is selected.
/// </remarks>
public sealed class ChaosPolicy
{
    /// <summary>A policy that never injects chaos (e.g. wrong environment).</summary>
    public static readonly ChaosPolicy Disabled = new([]);

    /// <summary>The list of rules to evaluate.</summary>
    private readonly IReadOnlyList<ChaosRule> _rules;

    /// <summary>Initializes a new instance of the <see cref="ChaosPolicy"/> class.</summary>
    /// <param name="rules">The list of rules to evaluate.</param>
    internal ChaosPolicy(IReadOnlyList<ChaosRule> rules)
    {
        // Sort rules by pattern length descending to ensure more specific rules match first
        _rules = rules
            .OrderByDescending(r => r.Pattern.Length)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Evaluates the policy against the given HTTP context.
    /// Returns a <see cref="ChaosRule"/> if chaos should be injected; otherwise, <c>null</c>.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A <see cref="ChaosRule"/> instance if a match is found and probability is met; otherwise, <c>null</c>.</returns>
    internal ChaosRule? Evaluate(HttpContext context)
    {
        foreach (var rule in _rules)
        {
            if (!rule.Matches(context))
                continue;

            if (rule.ShouldFire())
                return rule;
        }

        return null;
    }
}
