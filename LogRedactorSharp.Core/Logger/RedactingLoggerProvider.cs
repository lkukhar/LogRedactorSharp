using Microsoft.Extensions.Logging;

namespace LogRedactorSharp.Core.Logger;

/// <summary>
/// Redacting Logger Provider
/// </summary>
public sealed class RedactingLoggerProvider : ILoggerProvider
{
    private readonly ILoggerProvider _loggerProvider;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="loggerProvider"></param>
    public RedactingLoggerProvider(ILoggerProvider loggerProvider) =>
        _loggerProvider = loggerProvider;

    /// <summary>
    /// Default dispose
    /// </summary>
    public void Dispose() => _loggerProvider.Dispose();

    /// <summary>
    /// Create Logger
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
    {
        var logger = _loggerProvider.CreateLogger(categoryName);
        return new RedactingLogger(logger);
    }
}
