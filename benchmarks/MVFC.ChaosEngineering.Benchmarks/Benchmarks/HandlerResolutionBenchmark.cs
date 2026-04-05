namespace MVFC.ChaosEngineering.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob]
public class HandlerResolutionBenchmark
{
    [Benchmark]
    public void ResolveLatency() =>
        ChaosHandlerRegistry.GetHandler(ChaosKind.Latency);

    [Benchmark]
    public void ResolveException() =>
        ChaosHandlerRegistry.GetHandler(ChaosKind.Exception);

    [Benchmark]
    public void ResolveStatusCode() =>
        ChaosHandlerRegistry.GetHandler(ChaosKind.StatusCode);

    [Benchmark]
    public void ResolveBandwidth() =>
        ChaosHandlerRegistry.GetHandler(ChaosKind.BandwidthThrottle);

    [Benchmark]
    public void ResolveUnknown() =>
        ChaosHandlerRegistry.GetHandler((ChaosKind)999);
}
