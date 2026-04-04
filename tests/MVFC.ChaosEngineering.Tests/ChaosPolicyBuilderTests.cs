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

        policy.Evaluate(HostHelper.CreateContext("/api")).ShouldInject.Should().BeTrue();
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

        policy.Evaluate(HostHelper.CreateContext("/api")).ShouldInject.Should().BeTrue();
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
        policy.Evaluate(HostHelper.CreateContext("/api")).ShouldInject.Should().BeFalse();
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

        policy.Evaluate(HostHelper.CreateContext("/api")).ShouldInject.Should().BeTrue();
    }

    [Fact]
    public void WithProbability_BeforeForRoute_ThrowsInvalidOperationException()
    {
        var builder = new ChaosPolicyBuilder();
        var act = () => builder.WithProbability(0.5);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MultipleRules_FirstMatchWins()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/**").WithProbability(1.0).WithStatusCode(503)
            .ForRoute("/api/orders").WithProbability(1.0).WithKind(ChaosKind.Abort)
            .Build();

        // /api/orders matches both, but wildcard rule is first
        var decision = policy.Evaluate(HostHelper.CreateContext("/api/orders"));
        decision.Kind.Should().Be(ChaosKind.StatusCode);
    }
}
