namespace MVFC.ChaosEngineering.Policy;

/// <summary>Represents a collection of chaos rules that are evaluated against incoming requests.</summary>
public sealed class ChaosPolicy
{
    /// <summary>A policy that never injects chaos (e.g. wrong environment).</summary>
    public static readonly ChaosPolicy Disabled = new([]);

    private readonly IReadOnlyList<ChaosRule> _rules;

    /// <summary>Initializes a new instance of the <see cref="ChaosPolicy"/> class.</summary>
    /// <param name="rules">The list of rules to evaluate.</param>
    internal ChaosPolicy(IReadOnlyList<ChaosRule> rules)
    {
        _rules = rules;
    }

    /// <summary>
    /// Evaluates the policy against the given HTTP context.
    /// Returns a <see cref="ChaosDecision"/> indicating whether and what chaos should be injected.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    internal ChaosDecision Evaluate(HttpContext context)
    {
        var rule = _rules.FirstOrDefault(r => r.Matches(context));

        if (rule is null)
            return ChaosDecision.None;

        if (!rule.ShouldFire())
            return ChaosDecision.None;

        return ChaosDecision.FromRule(rule);
    }
}
