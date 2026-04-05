namespace MVFC.ChaosEngineering.Tests;

public sealed class ChaosPolicyBuilderTests
{
    [Fact]
    public void Build_WithoutForRoute_ThrowsWhenConfiguringRule()
    {
        var builder = new ChaosPolicyBuilder();

        var act = () => builder.WithProbability(0.5);

        act.Should().Throw<InvalidOperationException>().WithMessage("*ForRoute*");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(double.NaN)]
    public void Build_InvalidProbability_Throws(double probability)
    {
        var act = () => new ChaosPolicyBuilder()
            .ForRoute("/api/test")
            .WithProbability(probability)
            .Build();

        act.Should().Throw<InvalidOperationException>().WithMessage("*Probability*");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void Build_ValidProbability_DoesNotThrow(double probability)
    {
        var act = () => new ChaosPolicyBuilder()
            .ForRoute("/api/test")
            .WithProbability(probability)
            .WithStatusCode(503)
            .Build();

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-9999)]
    public void Build_BandwidthThrottle_InvalidBytesPerSecond_Throws(int bytesPerSecond)
    {
        var act = () => new ChaosPolicyBuilder()
            .ForRoute("/api/test")
            .WithBandwidthThrottle(bytesPerSecond)
            .Build();

        act.Should().Throw<InvalidOperationException>().WithMessage("*BytesPerSecond*");
    }

    [Fact]
    public void Build_BandwidthThrottle_ValidBytesPerSecond_DoesNotThrow()
    {
        var act = () => new ChaosPolicyBuilder()
            .ForRoute("/api/test")
            .WithBandwidthThrottle(bytesPerSecond: 1)
            .Build();

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Build_PartialResponse_InvalidPartialBytes_Throws(int partialBytes)
    {
        var act = () => new ChaosPolicyBuilder()
            .ForRoute("/api/test")
            .WithPartialResponse(partialBytes)
            .Build();

        act.Should().Throw<InvalidOperationException>().WithMessage("*PartialBytes*");
    }

    [Fact]
    public void Build_PartialResponse_ValidPartialBytes_DoesNotThrow()
    {
        var act = () => new ChaosPolicyBuilder()
            .ForRoute("/api/test")
            .WithPartialResponse(partialBytes: 1)
            .Build();

        act.Should().NotThrow();
    }

    [Fact]
    public void Build_RandomLatency_MinGreaterThanMax_Throws()
    {
        var act = () => new ChaosPolicyBuilder()
            .ForRoute("/api/test")
            .WithRandomLatency(
                min: TimeSpan.FromMilliseconds(500),
                max: TimeSpan.FromMilliseconds(100))
            .Build();

        act.Should().Throw<InvalidOperationException>().WithMessage("*MinLatency*MaxLatency*");
    }

    [Fact]
    public void Build_RandomLatency_MinEqualsMax_DoesNotThrow()
    {
        var ts = TimeSpan.FromMilliseconds(200);

        var act = () => new ChaosPolicyBuilder()
            .ForRoute("/api/test")
            .WithRandomLatency(min: ts, max: ts)
            .Build();

        act.Should().NotThrow();
    }

    [Fact]
    public void Build_ErrorMessage_IncludesRouteName()
    {
        const string ROUTE = "/api/payments";

        var act = () => new ChaosPolicyBuilder()
            .ForRoute(ROUTE)
            .WithBandwidthThrottle(bytesPerSecond: -1)
            .Build();

        act.Should().Throw<InvalidOperationException>().WithMessage($"*{ROUTE}*");
    }

    [Fact]
    public void Build_MultipleRules_ValidatesAll_ThrowsOnSecondInvalidRule()
    {
        var act = () => new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .WithProbability(1.0)
            .WithStatusCode(503)
            .ForRoute("/api/payments")          // regra 2 inválida
            .WithBandwidthThrottle(bytesPerSecond: 0)
            .Build();

        act.Should().Throw<InvalidOperationException>().WithMessage("*BytesPerSecond*payments*");
    }

    [Fact]
    public void Build_NoEnvironmentFilter_AlwaysBuildsActivePolicy()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api")
            .WithProbability(1.0)
            .Build();

        policy.Evaluate(HttpClientHelper.CreateContext("/api")).Should().NotBeNull();
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

        policy.Evaluate(HttpClientHelper.CreateContext("/api")).Should().NotBeNull();
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
        policy.Evaluate(HttpClientHelper.CreateContext("/api")).Should().BeNull();
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

        policy.Evaluate(HttpClientHelper.CreateContext("/api")).Should().NotBeNull();
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

        var rule = policy.Evaluate(HttpClientHelper.CreateContext("/api/orders"));
        rule.Should().NotBeNull();
        rule!.Kind.Should().Be(ChaosKind.Abort);
    }
}
