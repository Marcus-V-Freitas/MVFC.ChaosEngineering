namespace MVFC.ChaosEngineering.Benchmarks.Helpers;

internal static class HostHelper
{
    internal const string PATTERN = "/api/orders";

    internal static IHost BuildHost(Action<IApplicationBuilder> configureChaos)
    {
        return new HostBuilder()
            .ConfigureWebHost(webBuilder => webBuilder
                .UseTestServer()
                .ConfigureServices(s => s.AddRouting())
                .Configure(app =>
                {
                    configureChaos(app);
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapGet(PATTERN, async ctx =>
                    {
                        ctx.Response.StatusCode = 200;
                        await ctx.Response.WriteAsync("OK").ConfigureAwait(false);
                    }));
                }))
            .Build();
    }
}
