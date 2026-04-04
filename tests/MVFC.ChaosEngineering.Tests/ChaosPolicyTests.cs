namespace MVFC.ChaosEngineering.Tests;

public sealed class ChaosPolicyTests
{
    [Fact]
    public void Evaluate_NoMatchingRule_ReturnsNull()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .Build();

        var rule = policy.Evaluate(HostHelper.CreateContext("/api/payments"));

        rule.Should().BeNull();
    }

    [Fact]
    public void Evaluate_MatchingRuleProbabilityOne_ReturnsRule()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .WithProbability(1.0)
            .WithKind(ChaosKind.Exception)
            .Build();

        var rule = policy.Evaluate(HostHelper.CreateContext("/api/orders"));

        rule.Should().NotBeNull();
        rule!.Kind.Should().Be(ChaosKind.Exception);
    }

    [Fact]
    public void Evaluate_MatchingRuleProbabilityZero_ReturnsNull()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .WithProbability(0.0)
            .Build();

        var rule = policy.Evaluate(HostHelper.CreateContext("/api/orders"));

        rule.Should().BeNull();
    }

    [Fact]
    public void Evaluate_WildcardPattern_MatchesSubPaths()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/**")
            .WithProbability(1.0)
            .WithLatency(TimeSpan.FromMilliseconds(500))
            .Build();

        var rule = policy.Evaluate(HostHelper.CreateContext("/api/orders/123"));

        rule.Should().NotBeNull();
        rule!.Kind.Should().Be(ChaosKind.Latency);
        rule.Latency.Should().Be(TimeSpan.FromMilliseconds(500));
    }

    [Fact]
    public void Evaluate_StatusCodeRule_ReturnsCorrectStatusCode()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/products")
            .WithProbability(1.0)
            .WithStatusCode(503)
            .Build();

        var rule = policy.Evaluate(HostHelper.CreateContext("/api/products"));

        rule.Should().NotBeNull();
        rule!.Kind.Should().Be(ChaosKind.StatusCode);
        rule.StatusCode.Should().Be(503);
    }

    [Fact]
    public void Evaluate_CustomExceptionType_ReturnsCorrectType()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/payments")
            .WithProbability(1.0)
            .WithException<TimeoutException>()
            .Build();

        var rule = policy.Evaluate(HostHelper.CreateContext("/api/payments"));

        rule.Should().NotBeNull();
        rule!.ExceptionType.Should().Be<TimeoutException>();
    }

    [Fact]
    public void Evaluate_WithHeaderFilter_MatchesOnlyIfHeaderPresent()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .WithRequestHeader("X-Chaos", "true")
            .WithProbability(1.0)
            .WithStatusCode(500)
            .Build();

        // No header
        policy.Evaluate(HostHelper.CreateContext("/api/orders")).Should().BeNull();

        // Wrong header value
        policy.Evaluate(HostHelper.CreateContext("/api/orders", "X-Chaos", "false")).Should().BeNull();

        // Correct header
        policy.Evaluate(HostHelper.CreateContext("/api/orders", "X-Chaos", "true")).Should().NotBeNull();
    }

    [Fact]
    public void Disabled_NeverInjectsForAnyPath()
    {
        var rule = ChaosPolicy.Disabled.Evaluate(HostHelper.CreateContext("/api/anything"));
        rule.Should().BeNull();
    }
}
