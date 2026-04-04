var builder = WebApplication.CreateBuilder(args);

// Register chaos engineering services with a default policy.
// This enables IOptionsMonitor support and structured logging.
builder.Services.AddChaos(policy =>
{
    policy
        .ForRoute("/api/orders/**")
        .WithProbability(1.0)
        .WithStatusCode(503);

    policy
        .ForRoute("/api/payments/**")
        .WithProbability(1.0)
        .WithKind(ChaosKind.Exception);

    policy
        .ForRoute("/api/slow")
        .WithProbability(1.0)
        .WithLatency(TimeSpan.FromMilliseconds(200));

    policy
        .ForRoute("/api/unstable")
        .WithProbability(1.0)
        .WithKind(ChaosKind.RandomStatusCode);

    policy
        .ForRoute("/api/timeout")
        .WithProbability(1.0)
        .WithKind(ChaosKind.Timeout);

    policy
        .ForRoute("/api/header-chaos")
        .WithProbability(1.0)
        .WithKind(ChaosKind.HeaderInjection)
        .WithHeader("X-Chaos-Scenario", "payments-latency");

    policy
        .ForRoute("/api/throttle")
        .WithProbability(1.0)
        .WithThrottle(TimeSpan.FromSeconds(10));

    policy
        .ForRoute("/api/corrupt-body")
        .WithProbability(1.0)
        .WithCorruptBody();

    policy
        .ForRoute("/api/empty-body")
        .WithProbability(1.0)
        .WithEmptyBody();

    policy
        .ForRoute("/api/slow-body")
        .WithProbability(1.0)
        .WithSlowBody(TimeSpan.FromMilliseconds(150), chunkSize: 32);

    policy
        .ForRoute("/api/redirect")
        .WithProbability(1.0)
        .WithForcedRedirect("/api/products");

    policy
        .ForRoute("/api/random-latency")
        .WithProbability(1.0)
        .WithRandomLatency(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(500));

    policy
        .ForRoute("/api/partial")
        .WithProbability(1.0)
        .WithPartialResponse(32);

    policy
        .ForRoute("/api/bandwidth")
        .WithProbability(1.0)
        .WithBandwidthThrottle(bytesPerSecond: 128);

    policy
        .ForRoute("/api/wrong-content-type")
        .WithProbability(1.0)
        .WithContentTypeCorruption("text/plain");

    policy
        .ForRoute("/api/abort")
        .WithProbability(1.0)
        .WithKind(ChaosKind.Abort);

    // Demonstration of the new Header Filtering feature.
    // This rule only fires if the request includes 'X-Chaos: true' header.
    policy
        .ForRoute("/api/filtered")
        .WithRequestHeader("X-Chaos", "true")
        .WithProbability(1.0)
        .WithStatusCode(500);
});

var app = builder.Build();

// Activate chaos middleware using the policy registered in DI.
app.UseChaos();

// Map all demonstration endpoints from the extension class.
app.MapChaosEndpoints();

await app.RunAsync().ConfigureAwait(false);
