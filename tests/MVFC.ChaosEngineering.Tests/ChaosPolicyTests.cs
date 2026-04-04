namespace MVFC.ChaosEngineering.Tests;

public sealed class ChaosPolicyTests
{
    [Fact]
    public void Evaluate_NoMatchingRule_ReturnsNoneDecision()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .Build();

        var decision = policy.Evaluate(HostHelper.CreateContext("/api/payments"));

        decision.ShouldInject.Should().BeFalse();
    }

    [Fact]
    public void Evaluate_MatchingRuleProbabilityOne_ReturnsInjectDecision()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .WithProbability(1.0)
            .WithKind(ChaosKind.Exception)
            .Build();

        var decision = policy.Evaluate(HostHelper.CreateContext("/api/orders"));

        decision.ShouldInject.Should().BeTrue();
        decision.Kind.Should().Be(ChaosKind.Exception);
    }

    [Fact]
    public void Evaluate_MatchingRuleProbabilityZero_ReturnsNoneDecision()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .WithProbability(0.0)
            .Build();

        var decision = policy.Evaluate(HostHelper.CreateContext("/api/orders"));

        decision.ShouldInject.Should().BeFalse();
    }

    [Fact]
    public void Evaluate_WildcardPattern_MatchesSubPaths()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/**")
            .WithProbability(1.0)
            .WithLatency(TimeSpan.FromMilliseconds(500))
            .Build();

        var decision = policy.Evaluate(HostHelper.CreateContext("/api/orders/123"));

        decision.ShouldInject.Should().BeTrue();
        decision.Kind.Should().Be(ChaosKind.Latency);
        decision.Latency.Should().Be(TimeSpan.FromMilliseconds(500));
    }

    [Fact]
    public void Evaluate_StatusCodeRule_ReturnsCorrectStatusCode()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/products")
            .WithProbability(1.0)
            .WithStatusCode(503)
            .Build();

        var decision = policy.Evaluate(HostHelper.CreateContext("/api/products"));

        decision.ShouldInject.Should().BeTrue();
        decision.Kind.Should().Be(ChaosKind.StatusCode);
        decision.StatusCode.Should().Be(503);
    }

    [Fact]
    public void Evaluate_CustomExceptionType_ReturnsCorrectType()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/payments")
            .WithProbability(1.0)
            .WithException<TimeoutException>()
            .Build();

        var decision = policy.Evaluate(HostHelper.CreateContext("/api/payments"));

        decision.ShouldInject.Should().BeTrue();
        decision.ExceptionType.Should().Be<TimeoutException>();
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
        policy.Evaluate(HostHelper.CreateContext("/api/orders")).ShouldInject.Should().BeFalse();

        // Wrong header value
        policy.Evaluate(HostHelper.CreateContext("/api/orders", "X-Chaos", "false")).ShouldInject.Should().BeFalse();

        // Correct header
        policy.Evaluate(HostHelper.CreateContext("/api/orders", "X-Chaos", "true")).ShouldInject.Should().BeTrue();
    }

    [Fact]
    public void Disabled_NeverInjectsForAnyPath()
    {
        var decision = ChaosPolicy.Disabled.Evaluate(HostHelper.CreateContext("/api/anything"));
        decision.ShouldInject.Should().BeFalse();
    }
}
