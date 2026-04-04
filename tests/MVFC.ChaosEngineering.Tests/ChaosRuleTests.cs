namespace MVFC.ChaosEngineering.Tests;

public sealed class ChaosRuleTests
{
    [Theory]
    [InlineData("/api/orders", "/api/orders", true)]
    [InlineData("/API/ORDERS", "/api/orders", true)]
    [InlineData("/api/orders/123", "/api/orders", false)]
    [InlineData("/api/orders/123", "/api/orders/**", true)]
    [InlineData("/api/orders", "/api/orders/**", false)]
    [InlineData("/api/payments/pix/123", "/api/payments/**", true)]
    [InlineData("/other", "/api/**", false)]
    public void Matches_ShouldRespectPatternSemantics(string path, string pattern, bool expected)
    {
        var rule = new ChaosRule(Pattern: pattern);
        rule.Matches(HostHelper.CreateContext(path)).Should().Be(expected);
    }

    [Fact]
    public void Matches_EmptyPattern_ReturnsFalse()
    {
        var rule = new ChaosRule(Pattern: string.Empty);
        rule.Matches(HostHelper.CreateContext("/api/anything")).Should().BeFalse();
    }

    [Fact]
    public void Matches_WhitespacePattern_ReturnsFalse()
    {
        var rule = new ChaosRule(Pattern: "   ");
        rule.Matches(HostHelper.CreateContext("/api/anything")).Should().BeFalse();
    }

    [Fact]
    public void ShouldFire_ProbabilityZero_NeverFires()
    {
        var rule = new ChaosRule(Pattern: "/api", Probability: 0.0);
        for (var i = 0; i < 1000; i++)
            rule.ShouldFire().Should().BeFalse();
    }

    [Fact]
    public void ShouldFire_ProbabilityOne_AlwaysFires()
    {
        var rule = new ChaosRule(Pattern: "/api", Probability: 1.0);
        for (var i = 0; i < 100; i++)
            rule.ShouldFire().Should().BeTrue();
    }
}
