namespace MVFC.ChaosEngineering.Benchmarks.Helpers;

internal static class FakeHttpContextFactory
{
    internal static HttpContext Create(string path, string? headerName = null, string? headerValue = null)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;

        if (headerName is not null)
            context.Request.Headers[headerName] = headerValue ?? string.Empty;

        return context;
    }
}
