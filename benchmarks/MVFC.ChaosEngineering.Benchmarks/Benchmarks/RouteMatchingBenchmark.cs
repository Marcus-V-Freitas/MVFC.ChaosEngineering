namespace MVFC.ChaosEngineering.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob]
public class RouteMatchingBenchmark
{
    private ChaosRule _exactRule = null!;
    private ChaosRule _wildcardRule = null!;
    private ChaosRule _headerRule = null!;

    private HttpContext _exactContext = null!;
    private HttpContext _wildcardContext = null!;
    private HttpContext _noMatchContext = null!;
    private HttpContext _headerContext = null!;

    [GlobalSetup]
    public void Setup()
    {
        _exactRule = new ChaosRule(Pattern: "/api/orders");
        _wildcardRule = new ChaosRule(Pattern: "/api/payments/**");
        _headerRule = new ChaosRule(Pattern: "/api/orders", HeaderName: "X-Chaos", HeaderValue: "true");

        _exactContext = FakeHttpContextFactory.Create("/api/orders");
        _wildcardContext = FakeHttpContextFactory.Create("/api/payments/123/items");
        _noMatchContext = FakeHttpContextFactory.Create("/api/products/999");
        _headerContext = FakeHttpContextFactory.Create("/api/orders", "X-Chaos", "true");
    }

    [Benchmark(Baseline = true)]
    public bool ExactMatch() =>
        _exactRule.Matches(_exactContext);

    [Benchmark]
    public bool WildcardMatch() =>
        _wildcardRule.Matches(_wildcardContext);

    [Benchmark]
    public bool NoMatch() =>
        _exactRule.Matches(_noMatchContext);

    [Benchmark]
    public bool HeaderMatch() =>
        _headerRule.Matches(_headerContext);
}
