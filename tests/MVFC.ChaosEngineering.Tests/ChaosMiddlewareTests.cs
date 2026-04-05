namespace MVFC.ChaosEngineering.Tests;

public sealed class ChaosMiddlewareTests
{
    [Fact]
    public async Task Middleware_Probability_HonorsConfiguredRate()
    {
        const int ITERATIONS = 1000;
        const double TARGET_PROBABILITY = 0.5;
        const double TOLERANCE = 0.08;

        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .WithProbability(TARGET_PROBABILITY)
            .WithStatusCode(503)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var faultCount = 0;
        for (var i = 0; i < ITERATIONS; i++)
        {
            var response = await api.GetOrdersAsync(TestContext.Current.CancellationToken);
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                faultCount++;
        }

        var actualRate = (double)faultCount / ITERATIONS;
        actualRate.Should().BeApproximately(TARGET_PROBABILITY, TOLERANCE, $"expected ~{TARGET_PROBABILITY * 100}% fault rate, got {actualRate * 100:F1}%");
    }

    [Fact]
    public async Task Middleware_MultipleRules_FallsBackToWildcard_WhenFirstRuleDoesNotFire()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .WithProbability(0.0)        // nunca dispara
            .WithStatusCode(503)
            .ForRoute("/api/**")         // wildcard fallback
            .WithProbability(1.0)
            .WithStatusCode(500)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetOrdersAsync(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Middleware_DisabledPolicy_PassesThrough()
    {
        using var host = HostHelper.BuildHost(ChaosPolicy.Disabled);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetOrdersAsync(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Middleware_StatusCodeRule_ReturnsConfiguredStatusCode()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .WithProbability(1.0)
            .WithStatusCode(503)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetOrdersAsync(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Middleware_ExceptionRule_ReturnsError()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/payments")
            .WithProbability(1.0)
            .WithKind(ChaosKind.Exception)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var act = async () => await api.GetPaymentsAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Middleware_ExceptionFactory_ReturnsError()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/factory")
            .WithProbability(1.0)
            .WithException(ctx => new InvalidOperationException($"Path: {ctx.Request.Path}"))
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var act = async () => await api.GetFactoryAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Middleware_LatencyRule_IntroducesDelay()
    {
        var delay = TimeSpan.FromMilliseconds(200);
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/slow")
            .WithProbability(1.0)
            .WithLatency(delay)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var response = await api.GetSlowAsync(TestContext.Current.CancellationToken);
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.Elapsed.Should().BeGreaterThanOrEqualTo(delay);
    }

    [Fact]
    public async Task Middleware_NoMatchingRoute_PassesThrough()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/orders")
            .WithProbability(1.0)
            .WithStatusCode(500)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetProductsAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Middleware_RandomLatencyRule_IntroducesDelayWithinRange()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/random-latency")
            .WithProbability(1.0)
            .WithRandomLatency(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(300))
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var sw = Stopwatch.StartNew();
        var response = await api.GetRandomLatencyAsync(TestContext.Current.CancellationToken);
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.Elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(100));
        sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Middleware_RandomStatusCodeRule_Returns5xx()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/unstable")
            .WithProbability(1.0)
            .WithKind(ChaosKind.RandomStatusCode)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetUnstableAsync(TestContext.Current.CancellationToken);

        ((int)response.StatusCode).Should().BeGreaterThanOrEqualTo(500).And.BeLessThan(600);
    }

    [Fact]
    public async Task Middleware_TimeoutRule_HangsUntilCancelled()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/timeout")
            .WithProbability(1.0)
            .WithKind(ChaosKind.Timeout)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));
        var act = async () => await api.GetTimeoutAsync(cts.Token).ConfigureAwait(false);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task Middleware_HeaderInjectionRule_InjectsChaosHeaders()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/header-chaos")
            .WithProbability(1.0)
            .WithKind(ChaosKind.HeaderInjection)
            .WithHeader("X-Custom", "injected")
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetHeaderChaosAsync(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("X-Chaos-Injected");
        response.Headers.Should().ContainKey("X-Custom");
        response.Headers.GetValues("X-Custom").Should().ContainSingle(v => v == "injected");
    }

    [Fact]
    public async Task Middleware_ThrottleRule_Returns429WithRetryAfterHeader()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/throttle")
            .WithProbability(1.0)
            .WithThrottle(TimeSpan.FromSeconds(5))
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetThrottleAsync(TestContext.Current.CancellationToken);

        ((int)response.StatusCode).Should().Be(429);
        response.Headers.Should().ContainKey("Retry-After");
        response.Headers.GetValues("Retry-After").Should().ContainSingle(v => v == "5");
    }

    [Fact]
    public async Task Middleware_AbortRule_ReturnsError()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/abort")
            .WithProbability(1.0)
            .WithKind(ChaosKind.Abort)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var act = async () => await api.GetAbortAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Middleware_CorruptBodyRule_Returns200WithInvalidJson()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/corrupt-body")
            .WithProbability(1.0)
            .WithCorruptBody()
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetCorruptBodyAsync(TestContext.Current.CancellationToken);
        var body = response.Content;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var act = () => System.Text.Json.JsonDocument.Parse(body!);
        act.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public async Task Middleware_EmptyBodyRule_Returns200WithNoContent()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/empty-body")
            .WithProbability(1.0)
            .WithEmptyBody()
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetEmptyBodyAsync(TestContext.Current.CancellationToken);
        var body = response.Content;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().BeEmpty();
    }

    [Fact]
    public async Task Middleware_SlowBodyRule_StreamsWithDelay()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/slow-body")
            .WithProbability(1.0)
            .WithSlowBody(TimeSpan.FromMilliseconds(150), chunkSize: 8)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var sw = Stopwatch.StartNew();

        var response = await api.GetSlowBodyAsync(TestContext.Current.CancellationToken);

        await response.Content!.CopyToAsync(Stream.Null, TestContext.Current.CancellationToken);

        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.Elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(150));
    }

    [Fact]
    public async Task Middleware_ForcedRedirectRule_Returns302WithLocationHeader()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/redirect")
            .WithProbability(1.0)
            .WithForcedRedirect("/api/products")
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);

        var testServer = host.GetTestServer();
        var client = testServer.CreateClient();
        var apiRefit = RestService.For<IChaosApi>(client);

        var response = await apiRefit.GetRedirectAsync(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Found);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Be("/api/products");
    }

    [Fact]
    public async Task Middleware_PartialResponseRule_ReturnsError()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/partial")
            .WithProbability(1.0)
            .WithPartialResponse(16)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var act = async () => await api.GetPartialAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Middleware_BandwidthThrottleRule_LimitsResponseThroughput()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/bandwidth")
            .WithProbability(1.0)
            .WithBandwidthThrottle(bytesPerSecond: 4)
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var sw = Stopwatch.StartNew();

        var response = await api.GetBandwidthAsync(TestContext.Current.CancellationToken);

        await response.Content!.CopyToAsync(Stream.Null, TestContext.Current.CancellationToken);

        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.Elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(900));
    }

    [Fact]
    public async Task Middleware_ContentTypeCorruptionRule_SetsWrongContentType()
    {
        var policy = new ChaosPolicyBuilder()
            .ForRoute("/api/wrong-content-type")
            .WithProbability(1.0)
            .WithContentTypeCorruption("text/plain")
            .Build();

        using var host = HostHelper.BuildHost(policy);
        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetWrongContentTypeAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ContentHeaders?.ContentType?.MediaType.Should().Be("text/plain");
    }

    [Fact]
    public async Task UseChaos_WithFluentBuilder_InjectsChaos()
    {
        // This test covers the UseChaos(app, Action<ChaosPolicyBuilder> configure) extension method
        using var host = HostHelper.BuildHost(builder =>
            builder
                .ForRoute("/api/orders")
                .WithStatusCode(503)
        );

        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetOrdersAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        response.Headers.Contains("X-Chaos-Injected").Should().BeTrue();
    }

    [Fact]
    public async Task UseChaos_WithOptionsPattern_InjectsChaos()
    {
        // This test covers AddChaos, ChaosPolicyFactory, and UseChaos() without parameters
        using var host = new HostBuilder()
            .ConfigureWebHost(webBuilder => webBuilder
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddLogging();
                    services.AddChaos(builder => builder.ForRoute("/api/options").WithStatusCode(503));
                })
                .Configure(app =>
                {
                    app.UseChaos();
                    app.UseRouting();
                    app.UseEndpoints(e => e.MapGet("/api/options", async c => await c.Response.WriteAsync("OK").ConfigureAwait(false)));
                }))
            .Build();

        await host.StartAsync(TestContext.Current.CancellationToken);
        var api = RestService.For<IChaosApi>(host.GetTestClient());

        var response = await api.GetOptionsAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }
}
