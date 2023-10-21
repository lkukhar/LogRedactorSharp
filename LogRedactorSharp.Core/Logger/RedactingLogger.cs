using Microsoft.Extensions.Logging;

namespace LogRedactorSharp.Core.Logger;

public sealed class RedactingLogger : ILogger
{
    private readonly ILogger _logger;

    public RedactingLogger(ILogger logger) =>
        _logger = logger;

    public IDisposable BeginScope<TState>(TState state) where TState : notnull =>
        _logger.BeginScope(state)!;

    public bool IsEnabled(LogLevel logLevel) =>
        _logger.IsEnabled(logLevel);



    public void Log<TState>(
        LogLevel logLevel, 
        EventId eventId, 
        TState state, 
        Exception? exception, 
        Func<TState, Exception?, string> formatter)
    {
        throw new NotImplementedException();
    }
}
