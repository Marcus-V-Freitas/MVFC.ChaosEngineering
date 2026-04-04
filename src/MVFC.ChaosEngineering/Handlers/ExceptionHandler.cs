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
        ChaosRule rule,
        RequestDelegate next,
        ChaosInstrumentation instrumentation,
        string path)
    {
        var ex = rule.ExceptionFactory?.Invoke(context)
            ?? (rule.ExceptionType is not null
                ? (Activator.CreateInstance(rule.ExceptionType) as Exception)
                : null)
            ?? new ChaosException();

        throw ex;
    }
}
