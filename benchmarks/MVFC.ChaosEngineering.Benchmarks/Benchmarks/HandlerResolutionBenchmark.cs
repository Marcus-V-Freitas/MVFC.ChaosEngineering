namespace MVFC.ChaosEngineering.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob]
public class HandlerResolutionBenchmark
{
    private readonly IChaosHandlerRegistry _registry = new ChaosHandlerRegistry();

    [Benchmark]
    public void ResolveLatency() =>
        _registry.GetHandler(ChaosKind.Latency);

    [Benchmark]
    public void ResolveException() =>
        _registry.GetHandler(ChaosKind.Exception);

    [Benchmark]
    public void ResolveStatusCode() =>
        _registry.GetHandler(ChaosKind.StatusCode);

    [Benchmark]
    public void ResolveBandwidth() =>
        _registry.GetHandler(ChaosKind.BandwidthThrottle);

    [Benchmark]
    public void ResolveUnknown()
    {
        try { _registry.GetHandler((ChaosKind)999); }
        catch (NotSupportedException) { /* expected */ }
    }
}
