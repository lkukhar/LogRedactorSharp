using LogRedactorSharp.Core.Attributes;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;

namespace LogRedactorSharp.Core.Logger;

/// <summary>
/// Custom logger used to redact sensetive information by keying
/// on properties with the <see cref="RedactAttribute"/>
/// </summary>
internal sealed class RedactingLogger : ILogger
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    public RedactingLogger(ILogger logger) =>
        _logger = logger;

    /// <summary>
    /// Default begin scope implementation
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull =>
        _logger.BeginScope(state)!;

    /// <summary>
    /// Default IsEnabled implementation
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel logLevel) =>
        _logger.IsEnabled(logLevel);

    /// <summary>
    /// Default Implementation of Log
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <param name="formatter"></param>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (state is IReadOnlyList<KeyValuePair<string, object?>>)
        {
            _logger.Log(
                logLevel,
                eventId, 
                state, 
                exception,
                (o, e) => PrintNonRedactedProperties(o));
        }
        else
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }

    private string PrintNonRedactedProperties(object? obj)
    {
        var stringBuilder = new StringBuilder();
        return BuildNonRedactedProperties(stringBuilder, obj, 0).ToString();
    }

    private StringBuilder BuildNonRedactedProperties(StringBuilder stringBuilder, object? obj, int indent)
    {
        if(obj == null)
        {
            return stringBuilder;
        }

        var indentString = new string(' ', indent);
        var objType = obj.GetType();
        var nonRedeactedProperties = objType.GetProperties()
            .Where(e => e.GetCustomAttribute<RedactAttribute>(true) == null)
            .ToList();

        foreach (var property in nonRedeactedProperties)
        {
            stringBuilder.Append(Environment.NewLine);

            var propertyValue = property?.GetValue(obj, null);
            if (Convert.GetTypeCode(propertyValue) == TypeCode.Object)
            {
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append($"{indentString}{property?.Name}:");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(indentString + "{");
                BuildNonRedactedProperties(stringBuilder, propertyValue, indent + 2);
                stringBuilder.Append(indentString + "}");
            }
            else
            {
                stringBuilder.Append($"{indentString}{property?.Name}: {propertyValue}");
            }
        }

        return stringBuilder;
    }
}
