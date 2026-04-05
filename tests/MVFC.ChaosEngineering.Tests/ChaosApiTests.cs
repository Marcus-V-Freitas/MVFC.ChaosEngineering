namespace MVFC.ChaosEngineering.Tests;

public sealed class ChaosApiTests(AppHostFixture fixture) : IClassFixture<AppHostFixture>
{
    private readonly IChaosApi _api = fixture.Api;

    [Fact]
    public async Task Orders_WithChaosStatusCodeRule_ShouldReturnServiceUnavailable()
    {
        var response = await _api.GetOrderAsync("123", TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Products_ShouldPassThroughWithoutChaos()
    {
        var response = await _api.GetProductsAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SlowEndpoint_ShouldIntroduceMinimumDelay()
    {
        var delay = TimeSpan.FromMilliseconds(200);
        var sw = Stopwatch.StartNew();
        var response = await _api.GetSlowAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        sw.Stop();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.Elapsed.Should().BeGreaterThanOrEqualTo(delay);
    }

    [Fact]
    public async Task UnstableEndpoint_ShouldReturnRandom5xxStatusCodes()
    {
        var statuses = new HashSet<HttpStatusCode>();
        for (var i = 0; i < 20; i++)
        {
            var response = await _api.GetUnstableAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.InternalServerError,
                HttpStatusCode.BadGateway,
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.GatewayTimeout);
            statuses.Add(response.StatusCode);
        }
        statuses.Count.Should().BeGreaterThan(1);
    }

    [Fact]
    public async Task TimeoutEndpoint_ShouldCancelClientRequest()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));
        var act = async () => await _api.GetTimeoutAsync(cts.Token).ConfigureAwait(true);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task HeaderChaos_ShouldInjectChaosHeaders()
    {
        var response = await _api.GetHeaderChaosAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("X-Chaos-Injected");
        response.Headers.GetValues("X-Chaos-Scenario").Should().ContainSingle(v => v == "payments-latency");
    }

    [Fact]
    public async Task ThrottleEndpoint_ShouldReturn429WithRetryAfterMatchingConfiguration()
    {
        var response = await _api.GetThrottleAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be((HttpStatusCode)429);
        response.Headers.TryGetValues("Retry-After", out var values).Should().BeTrue();
        values!.Should().ContainSingle(v => v == "10");
    }

    [Fact]
    public async Task CorruptBody_ShouldReturn200WithInvalidJson()
    {
        var response = await _api.GetCorruptBodyAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = response.Content;
        var act = () => System.Text.Json.JsonDocument.Parse(body!);
        act.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public async Task EmptyBody_ShouldReturn200WithNoContent()
    {
        var response = await _api.GetEmptyBodyAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = response.Content;
        body.Should().BeEmpty();
    }

    [Fact]
    public async Task SlowBody_ShouldReturn200AndTakeAtLeastConfiguredChunkDelay()
    {
        var sw = Stopwatch.StartNew();

        var response = await _api.GetSlowBodyAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);

        await response.Content!.CopyToAsync(Stream.Null, TestContext.Current.CancellationToken).ConfigureAwait(true);

        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.Elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(150));
    }

    [Fact]
    public async Task ForcedRedirect_ShouldReturn302WithLocationHeader()
    {
        var response = await _api.GetRedirectAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.Found);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Be("/api/products");
    }

    [Fact]
    public async Task RandomLatency_ShouldReturn200WithinConfiguredRange()
    {
        var sw = Stopwatch.StartNew();
        var response = await _api.GetRandomLatencyAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        sw.Stop();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.Elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(50));
        sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task PartialResponse_ShouldReturnError()
    {
        var act = async () => await _api.GetPartialAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task BandwidthThrottle_ShouldReturn200AndTakeAtLeastOneSecond()
    {
        var sw = Stopwatch.StartNew();

        var response = await _api.GetBandwidthAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);

        await response.Content!.CopyToAsync(Stream.Null, TestContext.Current.CancellationToken).ConfigureAwait(true);

        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.Elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(900));
    }

    [Fact]
    public async Task ContentTypeCorruption_ShouldReturn200WithWrongContentType()
    {
        var response = await _api.GetWrongContentTypeAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ContentHeaders?.ContentType?.MediaType.Should().Be("text/plain");
    }

    [Fact]
    public async Task ExceptionEndpoint_ShouldReturnError()
    {
        var act = async () => await _api.GetExceptionAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task AbortEndpoint_ShouldReturnError()
    {
        var act = async () => await _api.GetAbortAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        await act.Should().ThrowAsync<Exception>();
    }
}
