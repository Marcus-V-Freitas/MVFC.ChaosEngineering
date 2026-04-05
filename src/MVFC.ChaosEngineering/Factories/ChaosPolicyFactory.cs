namespace MVFC.ChaosEngineering.Factories;

/// <summary>Factory for creating <see cref="ChaosPolicy"/> instances.</summary>
internal sealed class ChaosPolicyFactory(Action<ChaosPolicyBuilder> configure) : IOptionsFactory<ChaosPolicy>
{
    /// <summary>Creates a new <see cref="ChaosPolicy"/> instance.</summary>
    /// <param name="name">The name of the policy.</param>
    /// <returns>A new <see cref="ChaosPolicy"/> instance.</returns>
    public ChaosPolicy Create(string name)
    {
        var builder = new ChaosPolicyBuilder();
        configure(builder);
        return builder.Build();
    }
}
