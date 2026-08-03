using Microsoft.Extensions.Logging;

namespace WorkoutApp.Application.Tests.TestDoubles;

public sealed class FakeLogger<T> : ILogger<T>
{
    public List<(LogLevel Level, Exception? Exception, string Message)> LogEntries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        LogEntries.Add((logLevel, exception, formatter(state, exception)));
    }
}