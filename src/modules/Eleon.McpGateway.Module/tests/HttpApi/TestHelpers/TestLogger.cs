using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Eleon.McpGateway.Module.Test.HttpApi.TestHelpers;

/// <summary>
/// Test logger that captures log messages for assertion (verify no secrets).
/// </summary>
internal sealed class TestLogger<T> : ILogger<T>
{
    private readonly ConcurrentQueue<LogEntry> logs = new();

    public IReadOnlyList<LogEntry> Logs => logs.ToList().AsReadOnly();

    public IDisposable BeginScope<TState>(TState state) => NullDisposable.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        logs.Enqueue(new LogEntry
        {
            LogLevel = logLevel,
            EventId = eventId,
            Message = message,
            Exception = exception
        });
    }

    public void Clear()
    {
        while (logs.TryDequeue(out _)) { }
    }

    public bool ContainsSecret(string secret)
    {
        return logs.Any(log => log.Message.Contains(secret, StringComparison.OrdinalIgnoreCase));
    }

    public bool ContainsAnySecrets(IEnumerable<string> secrets)
    {
        return secrets.Any(secret => ContainsSecret(secret));
    }

    public record LogEntry
    {
        public LogLevel LogLevel { get; init; }
        public EventId EventId { get; init; }
        public string Message { get; init; } = string.Empty;
        public Exception? Exception { get; init; }
    }

    private sealed class NullDisposable : IDisposable
    {
        public static readonly NullDisposable Instance = new();
        public void Dispose() { }
    }
}

internal sealed class TestLoggerFactory : ILoggerFactory
{
    private readonly ConcurrentDictionary<Type, TestLogger<object>> loggers = new();

    public void AddProvider(ILoggerProvider provider) { }

    public ILogger CreateLogger(string categoryName)
    {
        var type = Type.GetType(categoryName) ?? typeof(object);
        return loggers.GetOrAdd(type, _ => new TestLogger<object>());
    }

    public ILogger<T> CreateLogger<T>()
    {
        return new TestLogger<T>();
    }

    public void Dispose() { }

    public TestLogger<object> GetLogger<T>()
    {
        return loggers.GetOrAdd(typeof(T), _ => new TestLogger<object>());
    }
}
