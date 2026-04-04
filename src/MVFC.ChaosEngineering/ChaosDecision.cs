namespace MVFC.ChaosEngineering;

/// <summary>
/// Represents the outcome of evaluating a <see cref="ChaosPolicy"/> against an incoming request.
/// </summary>
/// <param name="ShouldInject">Gets a value indicating whether chaos should be injected for the current request.</param>
/// <param name="Kind">Gets the kind of chaos that should be injected.</param>
/// <param name="Latency">Gets the fixed latency to inject, if <see cref="Kind"/> is <see cref="ChaosKind.Latency"/>.</param>
/// <param name="MinLatency">Gets the minimum random latency to inject, if <see cref="Kind"/> is <see cref="ChaosKind.RandomLatency"/>.</param>
/// <param name="MaxLatency">Gets the maximum random latency to inject, if <see cref="Kind"/> is <see cref="ChaosKind.RandomLatency"/>.</param>
/// <param name="StatusCode">Gets the HTTP status code to return, if applicable to the chaos kind.</param>
/// <param name="ExceptionType">Gets the type of exception to throw, if <see cref="Kind"/> is <see cref="ChaosKind.Exception"/>.</param>
/// <param name="Headers">Gets the collection of headers to inject, if <see cref="Kind"/> is <see cref="ChaosKind.HeaderInjection"/>.</param>
/// <param name="RetryAfter">Gets the value for the Retry-After header, if <see cref="Kind"/> is <see cref="ChaosKind.Throttle"/>.</param>
/// <param name="CorruptedBody">Gets the corrupted body content, if <see cref="Kind"/> is <see cref="ChaosKind.CorruptBody"/>.</param>
/// <param name="ChunkDelay">Gets the delay between chunks, if <see cref="Kind"/> is <see cref="ChaosKind.SlowBody"/>.</param>
/// <param name="ChunkSize">Gets the size of each chunk in bytes, if applicable.</param>
/// <param name="RedirectUrl">Gets the URL to redirect to, if <see cref="Kind"/> is <see cref="ChaosKind.ForcedRedirect"/>.</param>
/// <param name="RedirectStatusCode">Gets the status code for the redirect, if <see cref="Kind"/> is <see cref="ChaosKind.ForcedRedirect"/>.</param>
/// <param name="PartialBytes">Gets the number of bytes to include in a partial response, if <see cref="Kind"/> is <see cref="ChaosKind.PartialResponse"/>.</param>
/// <param name="BytesPerSecond">Gets the target throughput in bytes per second, if <see cref="Kind"/> is <see cref="ChaosKind.BandwidthThrottle"/>.</param>
/// <param name="CorruptContentType">Gets the corrupted Content-Type header value, if <see cref="Kind"/> is <see cref="ChaosKind.ContentTypeCorruption"/>.</param>
/// <param name="ExceptionFactory">Gets the dynamic exception factory, if <see cref="Kind"/> is <see cref="ChaosKind.Exception"/>.</param>
internal sealed record ChaosDecision(
    bool ShouldInject = false,
    ChaosKind Kind = ChaosKind.Exception,
    TimeSpan Latency = default,
    TimeSpan MinLatency = default,
    TimeSpan MaxLatency = default,
    int StatusCode = 0,
    Type? ExceptionType = null,
    IReadOnlyDictionary<string, string>? Headers = null,
    TimeSpan? RetryAfter = null,
    string? CorruptedBody = null,
    TimeSpan ChunkDelay = default,
    int ChunkSize = 0,
    string? RedirectUrl = null,
    int RedirectStatusCode = 0,
    int PartialBytes = 0,
    int BytesPerSecond = 0,
    string CorruptContentType = "text/plain",
    Func<HttpContext, Exception>? ExceptionFactory = null)
{
    /// <summary>Gets a decision that indicates no chaos should be injected.</summary>
    internal static readonly ChaosDecision None = new()
    {
        ShouldInject = false,
    };

    /// <summary>Creates a <see cref="ChaosDecision"/> from a <see cref="ChaosRule"/>.</summary>
    /// <param name="rule">The rule to convert.</param>
    /// <returns>A new <see cref="ChaosDecision"/>.</returns>
    internal static ChaosDecision FromRule(ChaosRule rule) => new()
    {
        ShouldInject = true,
        Kind = rule.Kind,
        Latency = rule.Latency,
        MinLatency = rule.MinLatency,
        MaxLatency = rule.MaxLatency,
        StatusCode = rule.StatusCode,
        ExceptionType = rule.ExceptionType,
        Headers = rule.Headers,
        RetryAfter = rule.RetryAfter,
        CorruptedBody = rule.CorruptedBody,
        ChunkDelay = rule.ChunkDelay,
        ChunkSize = rule.ChunkSize,
        RedirectUrl = rule.RedirectUrl,
        RedirectStatusCode = rule.RedirectStatusCode,
        PartialBytes = rule.PartialBytes,
        BytesPerSecond = rule.BytesPerSecond,
        CorruptContentType = rule.CorruptContentType,
        ExceptionFactory = rule.ExceptionFactory
    };
}
