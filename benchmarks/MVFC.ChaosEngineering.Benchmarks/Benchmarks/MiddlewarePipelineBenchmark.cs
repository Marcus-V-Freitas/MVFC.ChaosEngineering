namespace MVFC.ChaosEngineering.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[Config(typeof(InProcessConfig))]
public class MiddlewarePipelineBenchmark
{
    private sealed class InProcessConfig : ManualConfig
    {
        public InProcessConfig() =>
            AddJob(Job.Default
               .WithToolchain(InProcessNoEmitToolchain.Instance)
               .WithIterationCount(20)
               .WithWarmupCount(5));
    }

    private IHost _hostNoChaos = null!;
    private IHost _hostNoMatch = null!;
    private IHost _hostMatch = null!;

    private HttpClient _clientNoChaos = null!;
    private HttpClient _clientWithChaosNoMatch = null!;
    private HttpClient _clientWithChaosMatch = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        _hostNoChaos = HostHelper.BuildHost(app => { });
        await _hostNoChaos.StartAsync().ConfigureAwait(false);
        _clientNoChaos = _hostNoChaos.GetTestClient();

        _hostNoMatch = HostHelper.BuildHost(app => app.UseChaos(builder => builder
            .ForRoute("/api/payments/**")
            .WithProbability(1.0)
            .WithStatusCode(503)
            .Build()));
        await _hostNoMatch.StartAsync().ConfigureAwait(false);
        _clientWithChaosNoMatch = _hostNoMatch.GetTestClient();

        _hostMatch = HostHelper.BuildHost(app => app.UseChaos(builder => builder
            .ForRoute(HostHelper.PATTERN)
            .WithProbability(1.0)
            .WithStatusCode(500)
            .Build()));
        await _hostMatch.StartAsync().ConfigureAwait(false);
        _clientWithChaosMatch = _hostMatch.GetTestClient();
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        _clientNoChaos.Dispose();
        _clientWithChaosNoMatch.Dispose();
        _clientWithChaosMatch.Dispose();

        await _hostNoChaos.StopAsync().ConfigureAwait(false);
        await _hostNoMatch.StopAsync().ConfigureAwait(false);
        await _hostMatch.StopAsync().ConfigureAwait(false);

        _hostNoChaos.Dispose();
        _hostNoMatch.Dispose();
        _hostMatch.Dispose();
    }

    [Benchmark(Baseline = true)]
    public Task<HttpResponseMessage> NoChaos()
        => _clientNoChaos.GetAsync(HostHelper.PATTERN);

    [Benchmark]
    public Task<HttpResponseMessage> ChaosRegistered_NoMatch()
        => _clientWithChaosNoMatch.GetAsync(HostHelper.PATTERN);

    [Benchmark]
    public Task<HttpResponseMessage> ChaosRegistered_Match()
        => _clientWithChaosMatch.GetAsync(HostHelper.PATTERN);
}
