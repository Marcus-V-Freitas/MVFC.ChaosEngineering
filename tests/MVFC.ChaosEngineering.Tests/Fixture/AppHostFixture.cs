namespace MVFC.ChaosEngineering.Tests.Fixture;

public sealed class AppHostFixture : IAsyncLifetime
{
    private ProjectAppHost _appHost = default!;

    internal HttpClient Client { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        _appHost = new ProjectAppHost();
        await _appHost.StartAsync().ConfigureAwait(false);
        Client = _appHost.CreateHttpClient("chaos-api");
    }

    public async ValueTask DisposeAsync()
    {
        Client?.Dispose();
        await _appHost.DisposeAsync().ConfigureAwait(false);
    }
}
