namespace MVFC.ChaosEngineering.Policy;

/// <summary>Fluent builder for creating a <see cref="ChaosPolicy"/>.</summary>
public sealed class ChaosPolicyBuilder
{
    private readonly List<ChaosEnvironment> _environments = [];
    private readonly List<ChaosRule> _rules = [];
    private ChaosRule? _currentRule;
    private string? _aspNetCoreEnvironment;

    /// <summary>Configures the environments where this policy is active.</summary>
    /// <param name="environments">One or more <see cref="ChaosEnvironment"/> values.</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder OnEnvironments(params ChaosEnvironment[] environments)
    {
        if (environments is { Length: > 0 })
            _environments.AddRange(environments);

        return this;
    }

    /// <summary>Overrides the environment detecton logic (useful for testing).</summary>
    /// <param name="environment">The environment string.</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder WithEnvironmentOverride(string environment)
    {
        _aspNetCoreEnvironment = environment;

        return this;
    }

    /// <summary>Starts a new rule definition for the given route pattern.</summary>
    /// <param name="pattern">The URI route pattern to match.</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder ForRoute(string pattern)
    {
        _currentRule = new ChaosRule
        {
            Pattern = pattern,
        };
        _rules.Add(_currentRule);

        return this;
    }

    /// <summary>Sets the frequency of chaos injection for the current rule.</summary>
    /// <param name="probability">A value between 0.0 and 1.0.</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder WithProbability(double probability)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Probability = probability,
        });
    }

    /// <summary>Sets the kind of chaos for the current rule.</summary>
    /// <param name="kind">The <see cref="ChaosKind"/>.</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder WithKind(ChaosKind kind)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = kind,
        });
    }

    /// <summary>Configures a fixed latency for the current rule.</summary>
    /// <param name="latency">The duration of the artificial delay.</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder WithLatency(TimeSpan latency)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.Latency,
            Latency = latency,
        });
    }

    /// <summary>Configures a random latency rule within [<paramref name="min"/>, <paramref name="max"/>].</summary>
    public ChaosPolicyBuilder WithRandomLatency(TimeSpan min, TimeSpan max)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.RandomLatency,
            MinLatency = min,
            MaxLatency = max,
        });
    }

    /// <summary>Configures a status code to return for the current rule.</summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder WithStatusCode(int statusCode)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.StatusCode,
            StatusCode = statusCode,
        });
    }

    /// <summary>Configures an exception to throw for the current rule.</summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder WithException<TException>() where TException : Exception
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.Exception,
            ExceptionType = typeof(TException),
            ExceptionFactory = null,
        });
    }

    /// <summary>Configures a dynamic exception factory for the current rule.</summary>
    /// <param name="factory">A function that returns an exception based on the current context.</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder WithException(Func<HttpContext, Exception> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.Exception,
            ExceptionFactory = factory,
            ExceptionType = null,
        });
    }

    /// <summary>Adds a custom header to the response for the current rule.</summary>
    /// <param name="name">The header name.</param>
    /// <param name="value">The header value.</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder WithHeader(string name, string value)
    {
        EnsureCurrentRule();

        var existing = _currentRule!;
        var map = existing.Headers is null
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(existing.Headers, StringComparer.OrdinalIgnoreCase);
        map[name] = value;

        return Replace(_ => existing with
        {
            Headers = map,
        });
    }

    /// <summary>Scopes the current rule so it only matches if the request contains the specified header.</summary>
    /// <param name="name">The required header name.</param>
    /// <param name="value">The required header value (optional).</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder WithRequestHeader(string name, string? value = null)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            HeaderName = name,
            HeaderValue = value,
        });
    }

    /// <summary>Configures a throttle (429) response for the current rule.</summary>
    /// <param name="retryAfter">Optional TimeSpan for the <c>Retry-After</c> header.</param>
    /// <param name="statusCode">Optional HTTP status code (defaults to 429).</param>
    /// <returns>The builder instance.</returns>
    public ChaosPolicyBuilder WithThrottle(TimeSpan? retryAfter = null, int statusCode = StatusCodes.Status429TooManyRequests)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.Throttle,
            StatusCode = statusCode,
            RetryAfter = retryAfter,
        });
    }

    /// <summary>Returns HTTP 200 with a corrupted/invalid body. Optionally provide the body string.</summary>
    public ChaosPolicyBuilder WithCorruptBody(string? body = null)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.CorruptBody,
            CorruptedBody = body,
        });
    }

    /// <summary>Returns HTTP 200 with an empty body.</summary>
    public ChaosPolicyBuilder WithEmptyBody()
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.EmptyBody,
        });
    }

    /// <summary>Streams the response in chunks of <paramref name="chunkSize"/> bytes with <paramref name="chunkDelay"/> between each.</summary>
    public ChaosPolicyBuilder WithSlowBody(TimeSpan? chunkDelay = null, int chunkSize = 64)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.SlowBody,
            ChunkDelay = chunkDelay ?? TimeSpan.FromMilliseconds(200),
            ChunkSize = chunkSize,
        });
    }

    /// <summary>Redirects to <paramref name="url"/> using status code <paramref name="statusCode"/> (default 302).</summary>
    public ChaosPolicyBuilder WithForcedRedirect(string url, int statusCode = StatusCodes.Status302Found)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.ForcedRedirect,
            RedirectUrl = url,
            RedirectStatusCode = statusCode,
        });
    }

    /// <summary>Closes the connection after writing at most <paramref name="partialBytes"/> bytes of body.</summary>
    public ChaosPolicyBuilder WithPartialResponse(int partialBytes = 64)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.PartialResponse,
            PartialBytes = partialBytes,
        });
    }

    /// <summary>Limits response throughput to <paramref name="bytesPerSecond"/> bytes/s.</summary>
    public ChaosPolicyBuilder WithBandwidthThrottle(int bytesPerSecond = 1024)
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.BandwidthThrottle,
            BytesPerSecond = bytesPerSecond,
        });
    }

    /// <summary>Serves the real body with a corrupted Content-Type header (default "text/plain").</summary>
    public ChaosPolicyBuilder WithContentTypeCorruption(string contentType = "text/plain")
    {
        EnsureCurrentRule();

        return Replace(existing => existing with
        {
            Kind = ChaosKind.ContentTypeCorruption,
            CorruptContentType = contentType,
        });
    }

    /// <summary>Builds the final <see cref="ChaosPolicy"/> based on the configured rules and environments.</summary>
    /// <returns>A new <see cref="ChaosPolicy"/> instance, or <see cref="ChaosPolicy.Disabled"/> if the environment does not match.</returns>
    public ChaosPolicy Build()
    {
        if (_environments.Count == 0)
            return new ChaosPolicy(_rules.AsReadOnly());

        var currentEnv = _aspNetCoreEnvironment
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Production";

        var allowed = _environments.Any(e => e.Matches(currentEnv));

        return allowed ? new ChaosPolicy(_rules.AsReadOnly()) : ChaosPolicy.Disabled;
    }

    /// <summary>Replaces the last rule with an updated version.</summary>
    private ChaosPolicyBuilder Replace(Func<ChaosRule, ChaosRule> update)
    {
        var updated = update(_currentRule!);
        _rules[^1] = updated;
        _currentRule = updated;

        return this;
    }

    /// <summary>Ensures that a rule has been started with <see cref="ForRoute"/>.</summary>
    private void EnsureCurrentRule()
    {
        if (_currentRule is null)
            throw new InvalidOperationException("Call ForRoute() before configuring rule properties.");
    }
}
