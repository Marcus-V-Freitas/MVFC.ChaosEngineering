namespace MVFC.ChaosEngineering.Tests.Fixture;

public sealed class AppHostFixture : IAsyncLifetime
{
    private ProjectAppHost _appHost = default!;
    private HttpClient _client = default!;

    public IChaosApi Api { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        _appHost = new ProjectAppHost();

        await _appHost.StartAsync().ConfigureAwait(false);

        _client = _appHost.CreateClient();
        Api = RestService.For<IChaosApi>(_client);
    }

    public async ValueTask DisposeAsync()
    {
        _client?.Dispose();
        await _appHost.DisposeAsync().ConfigureAwait(false);
    }
}
