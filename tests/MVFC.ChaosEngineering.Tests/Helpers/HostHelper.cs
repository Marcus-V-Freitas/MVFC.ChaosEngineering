namespace MVFC.ChaosEngineering.Tests.Helpers;

internal static class HostHelper
{
    private const string PATTERN = "/{**path}";

    internal static DefaultHttpContext CreateContext(string path, string? headerName = null, string? headerValue = null)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;

        if (headerName != null)
        {
            context.Request.Headers[headerName] = headerValue ?? string.Empty;
        }

        return context;
    }

    internal static IHost BuildHost(ChaosPolicy policy) =>
        BuildHostInternal(app => app.UseChaos(policy));

    internal static IHost BuildHost(Action<ChaosPolicyBuilder> configure) =>
        BuildHostInternal(app => app.UseChaos(configure));

    private static IHost BuildHostInternal(Action<IApplicationBuilder> configureChaos)
    {
        return new HostBuilder()
            .ConfigureWebHost(webBuilder => webBuilder
                .UseTestServer()
                .ConfigureServices(s => s.AddRouting())
                .Configure(app =>
                {
                    configureChaos(app);
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/api/slow-body", async ctx =>
                        {
                            ctx.Response.StatusCode = 200;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.WriteAsync(new string('x', 64)).ConfigureAwait(false);
                        });

                        endpoints.MapGet("/api/bandwidth", async ctx =>
                        {
                            ctx.Response.StatusCode = 200;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.WriteAsync(new string('x', 16)).ConfigureAwait(false);
                        });

                        endpoints.MapGet(PATTERN, async ctx =>
                        {
                            ctx.Response.StatusCode = 200;
                            await ctx.Response.WriteAsync("OK").ConfigureAwait(false);
                        });
                    });
                }))
            .Build();
    }
}
