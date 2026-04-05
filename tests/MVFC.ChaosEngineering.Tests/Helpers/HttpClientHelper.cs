namespace MVFC.ChaosEngineering.Tests.Helpers;

internal static class HttpClientHelper
{
    internal static HttpClient CreateClient(this ProjectAppHost appHost)
    {
        var uri = appHost.GetEndpoint("chaos-api");
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
        };

        return new HttpClient(handler)
        {
            BaseAddress = uri,
        };
    }

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
}
