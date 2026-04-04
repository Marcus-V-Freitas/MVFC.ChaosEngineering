namespace MVFC.ChaosEngineering.Enums;

/// <summary>Defines common environments for chaos evaluation.</summary>
public enum ChaosEnvironment
{
    /// <summary>Local development machine.</summary>
    Local,

    /// <summary>Development environment.</summary>
    Development,

    /// <summary>Staging or QA environment.</summary>
    Staging,

    /// <summary>Production environment. Chaos is disabled by default here.</summary>
    Production,
}
