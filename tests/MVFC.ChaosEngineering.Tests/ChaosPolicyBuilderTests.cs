namespace MVFC.ChaosEngineering.Tests;

public sealed class ChaosPolicyBuilderTests
{
    [Fact]
    public void Build_NoEnvironmentFilter_AlwaysBuildsActivePolicy()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api")
            .WithProbability(1.0)
            .Build();

        policy.Evaluate(HostHelper.CreateContext("/api")).Should().NotBeNull();
    }

    [Fact]
    public void Build_EnvironmentMatches_BuildsActivePolicy()
    {
        var policy = new ChaosPolicyBuilder()
            .OnEnvironments(ChaosEnvironment.Development)
            .WithEnvironmentOverride("Development")
            .ForRoute("/api")
            .WithProbability(1.0)
            .Build();

        policy.Evaluate(HostHelper.CreateContext("/api")).Should().NotBeNull();
    }

    [Fact]
    public void Build_EnvironmentDoesNotMatch_ReturnsDisabledPolicy()
    {
        var policy = new ChaosPolicyBuilder()
            .OnEnvironments(ChaosEnvironment.Development)
            .WithEnvironmentOverride("Production")
            .ForRoute("/api")
            .WithProbability(1.0)
            .Build();

        policy.Should().BeSameAs(ChaosPolicy.Disabled);
        policy.Evaluate(HostHelper.CreateContext("/api")).Should().BeNull();
    }

    [Fact]
    public void Build_MultipleEnvironments_MatchesAny()
    {
        var policy = new ChaosPolicyBuilder()
            .OnEnvironments(ChaosEnvironment.Development, ChaosEnvironment.Staging)
            .WithEnvironmentOverride("Staging")
            .ForRoute("/api")
            .WithProbability(1.0)
            .Build();

        policy.Evaluate(HostHelper.CreateContext("/api")).Should().NotBeNull();
    }

    [Fact]
    public void WithProbability_BeforeForRoute_ThrowsInvalidOperationException()
    {
        var builder = new ChaosPolicyBuilder();
        var act = () => builder.WithProbability(0.5);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MultipleRules_SpecificMatchWinsRegardlessOfOrder()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/**").WithProbability(1.0).WithStatusCode(503)
            .ForRoute("/api/orders").WithProbability(1.0).WithKind(ChaosKind.Abort)
            .Build();

        // /api/orders (len 11) is more specific than /api/** (len 7)
        var rule = policy.Evaluate(HostHelper.CreateContext("/api/orders"));
        rule.Should().NotBeNull();
        rule!.Kind.Should().Be(ChaosKind.Abort);
    }
}
