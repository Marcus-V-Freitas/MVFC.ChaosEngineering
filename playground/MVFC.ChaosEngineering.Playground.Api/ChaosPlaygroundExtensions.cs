namespace MVFC.ChaosEngineering.Playground.Api;

/// <summary>
/// Extension methods for configuring chaos playground endpoints.
/// </summary>
internal static class ChaosPlaygroundExtensions
{
    /// <summary>
    /// Maps all chaos playground demonstration endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapChaosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapDomainEndpoints();
        endpoints.MapLatencyEndpoints();
        endpoints.MapBodyEndpoints();
        endpoints.MapFaultEndpoints();

        return endpoints;
    }

    /// <summary>
    /// Maps domain-specific mock endpoints.
    /// </summary>
    private static void MapDomainEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/orders/{id:int}", (int id) =>
            Results.Ok(new
            {
                Id = id,
                Status = "Ok",
            }));

        endpoints.MapGet("/api/payments/{id:int}", (int id) =>
            Results.Ok(new
            {
                Id = id,
                Status = "Paid",
            }));

        endpoints.MapGet("/api/products", () =>
            Results.Ok("Products"));
    }

    /// <summary>
    /// Maps latency, timeout, throttle, and bandwidth demonstration endpoints.
    /// </summary>
    private static void MapLatencyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/slow", async () =>
        {
            await Task.Delay(50).ConfigureAwait(false);
            return Results.Ok("Slow but successful");
        });

        endpoints.MapGet("/api/random-latency", () =>
            Results.Ok("Random latency target"));

        endpoints.MapGet("/api/throttle", () =>
            Results.Ok("Throttle target"));

        endpoints.MapGet("/api/timeout", () =>
            Results.Ok("This response should never be observed when chaos is enabled"));

        endpoints.MapGet("/api/bandwidth", () =>
            Results.Text(new string('x', 256), "text/plain"));
    }

    /// <summary>
    /// Maps payload and body corruption demonstration endpoints.
    /// </summary>
    private static void MapBodyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/corrupt-body", () =>
            Results.Ok(new
            {
                Data = "real",
            }));

        endpoints.MapGet("/api/empty-body", () =>
            Results.Ok("Empty body target"));

        endpoints.MapGet("/api/slow-body", () =>
            Results.Text(new string('x', 64), "text/plain"));

        endpoints.MapGet("/api/partial", () =>
            Results.Ok("Partial response target"));

        endpoints.MapGet("/api/wrong-content-type", () =>
            Results.Ok(new
            {
                Data = "real",
            }));
    }

    /// <summary>
    /// Maps fault injection, redirect, abort, and header filtering endpoints.
    /// </summary>
    private static void MapFaultEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/unstable", () =>
            Results.Ok("Unstable endpoint"));

        endpoints.MapGet("/api/header-chaos", () =>
            Results.Ok("Header chaos"));

        endpoints.MapGet("/api/redirect", () =>
            Results.Ok("Redirect target"));

        endpoints.MapGet("/api/abort", () =>
            Results.Ok("Abort target"));

        endpoints.MapGet("/api/filtered", () =>
            Results.Ok("This only fails if X-Chaos: true is passed"));
    }
}
