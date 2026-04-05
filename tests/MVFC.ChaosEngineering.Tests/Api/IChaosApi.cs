namespace MVFC.ChaosEngineering.Tests.Api;

public interface IChaosApi
{
    [Get("/api/orders/{id}")]
    public Task<ApiResponse<string>> GetOrderAsync(string id, CancellationToken ct = default);

    [Get("/api/orders")]
    public Task<ApiResponse<string>> GetOrdersAsync(CancellationToken ct = default);

    [Get("/api/products")]
    public Task<ApiResponse<string>> GetProductsAsync(CancellationToken ct = default);

    [Get("/api/slow")]
    public Task<ApiResponse<string>> GetSlowAsync(CancellationToken ct = default);

    [Get("/api/unstable")]
    public Task<ApiResponse<string>> GetUnstableAsync(CancellationToken ct = default);

    [Get("/api/timeout")]
    public Task<ApiResponse<string>> GetTimeoutAsync(CancellationToken ct = default);

    [Get("/api/header-chaos")]
    public Task<ApiResponse<string>> GetHeaderChaosAsync(CancellationToken ct = default);

    [Get("/api/throttle")]
    public Task<ApiResponse<string>> GetThrottleAsync(CancellationToken ct = default);

    [Get("/api/corrupt-body")]
    public Task<ApiResponse<string>> GetCorruptBodyAsync(CancellationToken ct = default);

    [Get("/api/empty-body")]
    public Task<ApiResponse<string>> GetEmptyBodyAsync(CancellationToken ct = default);

    [Get("/api/slow-body")]
    public Task<ApiResponse<Stream>> GetSlowBodyAsync(CancellationToken ct = default);

    [Get("/api/redirect")]
    public Task<ApiResponse<string>> GetRedirectAsync(CancellationToken ct = default);

    [Get("/api/random-latency")]
    public Task<ApiResponse<string>> GetRandomLatencyAsync(CancellationToken ct = default);

    [Get("/api/partial")]
    public Task<string> GetPartialAsync(CancellationToken ct = default);

    [Get("/api/bandwidth")]
    public Task<ApiResponse<Stream>> GetBandwidthAsync(CancellationToken ct = default);

    [Get("/api/wrong-content-type")]
    public Task<ApiResponse<string>> GetWrongContentTypeAsync(CancellationToken ct = default);

    [Get("/exception")]
    public Task<string> GetExceptionAsync(CancellationToken ct = default);

    [Get("/api/abort")]
    public Task<string> GetAbortAsync(CancellationToken ct = default);

    [Get("/api/factory")]
    public Task<ApiResponse<string>> GetFactoryAsync(CancellationToken ct = default);

    [Get("/api/options")]
    public Task<ApiResponse<string>> GetOptionsAsync(CancellationToken ct = default);

    [Get("/api/payments")]
    public Task<string> GetPaymentsAsync(CancellationToken ct = default);
}
