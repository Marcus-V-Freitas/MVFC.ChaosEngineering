namespace MVFC.ChaosEngineering.Enums;

/// <summary>Defines what kind of chaos will be injected when a rule matches.</summary>
public enum ChaosKind
{
    /// <summary>Throws an exception of the configured type.</summary>
    Exception,

    /// <summary>Introduces an artificial delay before continuing.</summary>
    Latency,

    /// <summary>Introduces a random delay within a configured range.</summary>
    RandomLatency,

    /// <summary>Overrides the HTTP response status code.</summary>
    StatusCode,

    /// <summary>Randomly chooses a 5xx HTTP status code.</summary>
    RandomStatusCode,

    /// <summary>Simulates an endless hang until the request is aborted.</summary>
    Timeout,

    /// <summary>Injects diagnostic chaos headers into the response.</summary>
    HeaderInjection,

    /// <summary>Simulates throttling with HTTP 429 and Retry-After.</summary>
    Throttle,

    /// <summary>Cancels the request via CancellationToken.</summary>
    Abort,

    /// <summary>Returns HTTP 200 with an invalid/corrupted body.</summary>
    CorruptBody,

    /// <summary>Returns HTTP 200 with an empty body (Content-Length: 0).</summary>
    EmptyBody,

    /// <summary>Streams the response in small chunks with a delay between each, simulating a slow network.</summary>
    SlowBody,

    /// <summary>Returns a redirect (301/302) to a configured URL.</summary>
    ForcedRedirect,

    /// <summary>Closes the connection mid-response before the body is fully written.</summary>
    PartialResponse,

    /// <summary>Limits response write throughput to a configured bytes-per-second rate.</summary>
    BandwidthThrottle,

    /// <summary>Returns the correct body but with a wrong Content-Type header.</summary>
    ContentTypeCorruption
}
