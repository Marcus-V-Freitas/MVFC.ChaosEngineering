namespace MVFC.ChaosEngineering.Diagnostics;

/// <summary>
/// Provides instrumentation for chaos engineering activities, including metrics for injected faults.
/// </summary>
internal sealed class ChaosInstrumentation : IDisposable
{
    private readonly Meter _meter;
    private readonly Counter<long> _faultsCounter;
    private readonly Counter<long> _requestsCounter;
    private readonly Histogram<double> _latencyHistogram;

    /// <summary>Initializes a new instance of the <see cref="ChaosInstrumentation"/> class.</summary>
    public ChaosInstrumentation()
    {
        _meter = new Meter("MVFC.ChaosEngineering", "1.0.0");

        _faultsCounter = _meter.CreateCounter<long>(
            "chaos.faults.injected",
            "{fault}",
            "The number of times chaos has been injected into a request.");

        _requestsCounter = _meter.CreateCounter<long>(
            "chaos.requests.evaluated",
            "{request}",
            "The total number of requests evaluated by the chaos middleware.");

        _latencyHistogram = _meter.CreateHistogram<double>(
            "chaos.latency.duration",
            "ms",
            "The actual duration of injected latency in milliseconds.");
    }

    /// <summary>Increments the counter for injected faults.</summary>
    /// <param name="kind">The kind of chaos injected.</param>
    /// <param name="route">The route pattern that matched.</param>
    public void RecordFault(string kind, string route)
    {
        _faultsCounter.Add(1,
            new KeyValuePair<string, object?>("chaos.kind", kind),
            new KeyValuePair<string, object?>("chaos.route", route));
    }

    /// <summary>Records the duration of injected latency in the histogram.</summary>
    /// <param name="ms">The duration in milliseconds.</param>
    /// <param name="kind">The kind of latency (fixed or random).</param>
    /// <param name="route">The route pattern that matched.</param>
    public void RecordLatency(double ms, string kind, string route)
    {
        _latencyHistogram.Record(ms,
            new KeyValuePair<string, object?>("chaos.kind", kind),
            new KeyValuePair<string, object?>("chaos.route", route));
    }

    /// <summary>Increments the counter for total evaluated requests.</summary>
    public void RecordEvaluation() =>
        _requestsCounter.Add(1);

    /// <inheritdoc/>
    public void Dispose() => _meter.Dispose();
}
