namespace MVFC.ChaosEngineering.Tests;

public sealed class ChaosApiTests(AppHostFixture fixture) : IClassFixture<AppHostFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task Orders_WithChaosStatusCodeRule_ShouldReturnServiceUnavailable()
    {
        var response = await _client.GetAsync("/api/orders/123", TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Products_ShouldPassThroughWithoutChaos()
    {
        var response = await _client.GetAsync("/api/products", TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SlowEndpoint_ShouldIntroduceMinimumDelay()
    {
        var delay = TimeSpan.FromMilliseconds(200);
        var sw = Stopwatch.StartNew();
        var response = await _client.GetAsync("/api/slow", TestContext.Current.CancellationToken).ConfigureAwait(true);
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
            var response = await _client.GetAsync("/api/unstable", TestContext.Current.CancellationToken).ConfigureAwait(true);
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
        var act = async () => await _client.GetAsync("/api/timeout", cts.Token).ConfigureAwait(true);
        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task HeaderChaos_ShouldInjectChaosHeaders()
    {
        var response = await _client.GetAsync("/api/header-chaos", TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("X-Chaos-Injected");
        response.Headers.GetValues("X-Chaos-Scenario").Should().ContainSingle(v => v == "payments-latency");
    }

    [Fact]
    public async Task ThrottleEndpoint_ShouldReturn429WithRetryAfterMatchingConfiguration()
    {
        var response = await _client.GetAsync("/api/throttle", TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be((HttpStatusCode)429);
        response.Headers.TryGetValues("Retry-After", out var values).Should().BeTrue();
        values!.Should().ContainSingle(v => v == "10");
    }

    [Fact]
    public async Task CorruptBody_ShouldReturn200WithInvalidJson()
    {
        var response = await _client.GetAsync("/api/corrupt-body", TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var act = () => System.Text.Json.JsonDocument.Parse(body);
        act.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public async Task EmptyBody_ShouldReturn200WithNoContent()
    {
        var response = await _client.GetAsync("/api/empty-body", TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.Should().BeEmpty();
    }

    [Fact]
    public async Task SlowBody_ShouldReturn200AndTakeAtLeastConfiguredChunkDelay()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/slow-body");

        var sw = Stopwatch.StartNew();

        var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, TestContext.Current.CancellationToken)
                                    .ConfigureAwait(true);

        await response.Content.CopyToAsync(Stream.Null, TestContext.Current.CancellationToken).ConfigureAwait(true);

        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.Elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(150));
    }

    [Fact]
    public async Task ForcedRedirect_ShouldReturn302WithLocationHeader()
    {
        using var noRedirectClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false })
        {
            BaseAddress = _client.BaseAddress
        };
        var response = await noRedirectClient.GetAsync("/api/redirect", TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.Found);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Be("/api/products");
    }

    [Fact]
    public async Task RandomLatency_ShouldReturn200WithinConfiguredRange()
    {
        var sw = Stopwatch.StartNew();
        var response = await _client.GetAsync("/api/random-latency", TestContext.Current.CancellationToken).ConfigureAwait(true);
        sw.Stop();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.Elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(50));
        sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task PartialResponse_ShouldAbortConnectionMidStream()
    {
        var act = async () => await _client.GetAsync("/api/partial", TestContext.Current.CancellationToken).ConfigureAwait(true);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
public async Task BandwidthThrottle_ShouldReturn200AndTakeAtLeastOneSecond()
{
    using var request = new HttpRequestMessage(HttpMethod.Get, "/api/bandwidth");

    var sw = Stopwatch.StartNew();

    var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, TestContext.Current.CancellationToken)
                                .ConfigureAwait(true);

    // Drena o stream — é aqui que o throttle acontece de verdade
    await response.Content.CopyToAsync(Stream.Null, TestContext.Current.CancellationToken).ConfigureAwait(true);

    sw.Stop();

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    sw.Elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(900));
}

    [Fact]
    public async Task ContentTypeCorruption_ShouldReturn200WithWrongContentType()
    {
        var response = await _client.GetAsync("/api/wrong-content-type", TestContext.Current.CancellationToken).ConfigureAwait(true);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/plain");
    }

    [Fact]
    public async Task ExceptionEndpoint_ShouldThrowException()
    {
        await _client.Invoking(async c =>
        {
            var r = await c.GetAsync("/exception").ConfigureAwait(false);
            r.EnsureSuccessStatusCode();
        }).Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task AbortEndpoint_ShouldCloseConnection()
    {
        var act = async () => await _client.GetAsync("/api/abort", TestContext.Current.CancellationToken).ConfigureAwait(true);
        await act.Should().ThrowAsync<Exception>();
    }
}
