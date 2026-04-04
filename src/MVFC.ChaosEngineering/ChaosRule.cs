namespace MVFC.ChaosEngineering;

/// <summary>
/// Positional record that defines a chaos rule with its matching pattern and injection parameters.
/// </summary>
/// <param name="Pattern">Gets the URI route pattern to match (e.g. "/api/orders" or "/api/payments/**").</param>
/// <param name="Probability">Gets the probability [0.0, 1.0] that chaos is injected when this rule matches. Default: 1.0</param>
/// <param name="Kind">Gets the kind of chaos that will be injected when this rule fires.</param>
/// <param name="Latency">Used when <see cref="Kind"/> is <see cref="ChaosKind.Latency"/>.</param>
/// <param name="MinLatency">Used when <see cref="Kind"/> is <see cref="ChaosKind.RandomLatency"/>.</param>
/// <param name="MaxLatency">Used when <see cref="Kind"/> is <see cref="ChaosKind.RandomLatency"/>.</param>
/// <param name="StatusCode">Used when <see cref="Kind"/> is <see cref="ChaosKind.StatusCode"/> or <see cref="ChaosKind.Throttle"/>.</param>
/// <param name="ExceptionType">Used when <see cref="Kind"/> is <see cref="ChaosKind.Exception"/>. Defaults to <see cref="ChaosException"/>.</param>
/// <param name="Headers">Optional headers to inject when <see cref="Kind"/> is <see cref="ChaosKind.HeaderInjection"/>.</param>
/// <param name="RetryAfter">Optional Retry-After value used when <see cref="Kind"/> is <see cref="ChaosKind.Throttle"/>.</param>
/// <param name="CorruptedBody">Body to write when <see cref="Kind"/> is <see cref="ChaosKind.CorruptBody"/>. Defaults to a malformed JSON string.</param>
/// <param name="ChunkDelay">Delay between chunks when <see cref="Kind"/> is <see cref="ChaosKind.SlowBody"/>.</param>
/// <param name="ChunkSize">Chunk size in bytes when <see cref="Kind"/> is <see cref="ChaosKind.SlowBody"/>.</param>
/// <param name="RedirectUrl">Redirect target URL when <see cref="Kind"/> is <see cref="ChaosKind.ForcedRedirect"/>.</param>
/// <param name="RedirectStatusCode">Redirect status code (301 or 302) when <see cref="Kind"/> is <see cref="ChaosKind.ForcedRedirect"/>. Default: 302.</param>
/// <param name="PartialBytes">Maximum bytes to write before aborting when <see cref="Kind"/> is <see cref="ChaosKind.PartialResponse"/>.</param>
/// <param name="BytesPerSecond">Bytes per second allowed when <see cref="Kind"/> is <see cref="ChaosKind.BandwidthThrottle"/>.</param>
/// <param name="CorruptContentType">Content-Type to inject when <see cref="Kind"/> is <see cref="ChaosKind.ContentTypeCorruption"/>. Defaults to "text/plain".</param>
/// <param name="HeaderName">Optional header name required for this rule to match.</param>
/// <param name="HeaderValue">Optional header value required for this rule to match.</param>
/// <param name="ExceptionFactory">Optional dynamic exception factory.</param>
internal sealed record ChaosRule(
    string Pattern = "",
    double Probability = 1.0,
    ChaosKind Kind = ChaosKind.Exception,
    TimeSpan Latency = default,
    TimeSpan MinLatency = default,
    TimeSpan MaxLatency = default,
    int StatusCode = 503,
    Type? ExceptionType = null,
    IReadOnlyDictionary<string, string>? Headers = null,
    TimeSpan? RetryAfter = null,
    string? CorruptedBody = null,
    TimeSpan ChunkDelay = default,
    int ChunkSize = 64,
    string? RedirectUrl = null,
    int RedirectStatusCode = StatusCodes.Status302Found,
    int PartialBytes = 64,
    int BytesPerSecond = 1024,
    string CorruptContentType = "text/plain",
    string? HeaderName = null,
    string? HeaderValue = null,
    Func<HttpContext, Exception>? ExceptionFactory = null)
{
    /// <summary>Checks if the given HTTP context matches the rule's criteria (pattern, environment, and headers).</summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns><c>true</c> if it matches; otherwise, <c>false</c>.</returns>
    internal bool Matches(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Pattern))
            return false;

        // Check path pattern
        bool pathMatch;
        if (Pattern.EndsWith("/**", StringComparison.Ordinal))
        {
            var prefix = Pattern[..^3];
            pathMatch = string.Equals(path, prefix, StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            pathMatch = string.Equals(path, Pattern, StringComparison.OrdinalIgnoreCase);
        }


        if (!pathMatch)
            return false;

        // Check required header if configured
        if (!string.IsNullOrWhiteSpace(HeaderName))
        {
            if (!context.Request.Headers.TryGetValue(HeaderName, out var values))
                return false;

            if (!string.IsNullOrWhiteSpace(HeaderValue) && !values.Contains(HeaderValue))
                return false;
        }

        return true;
    }

    /// <summary>Evaluates the configured probability to determine if the rule should fire.</summary>
    /// <returns><c>true</c> if it should fire; otherwise, <c>false</c>.</returns>
    internal bool ShouldFire() =>
        Random.Shared.NextDouble() < Probability;
}
