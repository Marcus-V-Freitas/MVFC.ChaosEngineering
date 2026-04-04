namespace MVFC.ChaosEngineering.Handlers;

/// <summary>
/// Handler that throws a configured exception or a default <see cref="ChaosException"/>.
/// </summary>
internal sealed class ExceptionHandler : IChaosHandler
{
    /// <inheritdoc />
    public ChaosKind Kind => ChaosKind.Exception;

    /// <inheritdoc />
    public Task HandleAsync(
        HttpContext context,
        ChaosDecision decision,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        var ex = decision.ExceptionFactory?.Invoke(context)
            ?? (decision.ExceptionType is not null
                ? (Activator.CreateInstance(decision.ExceptionType) as Exception)
                : null)
            ?? new ChaosException();

        throw ex;
    }
}
