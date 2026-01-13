using Eleon.Logging.Lib.VportalLogging;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Eleon.Logging.Lib.Tests;

internal sealed class RecordingOperationScopeFactory : IOperationScopeFactory
{
  public string? LastOperationName { get; private set; }
  public IReadOnlyDictionary<string, object?>? LastContext { get; private set; }

  public IDisposable Begin(string operationName, IReadOnlyDictionary<string, object?>? context = null)
  {
    LastOperationName = operationName;
    LastContext = context;
    return new NoopDisposable();
  }

  private sealed class NoopDisposable : IDisposable
  {
    public void Dispose()
    {
    }
  }
}

internal sealed class RecordingExceptionReporter : IExceptionReporter
{
  public Exception? LastException { get; private set; }
  public IReadOnlyDictionary<string, object?>? LastContext { get; private set; }
  public int CallCount { get; private set; }

  public void Report(Exception ex, IReadOnlyDictionary<string, object?>? context = null)
  {
    CallCount++;
    LastException = ex;
    LastContext = context;
  }

  public bool ShouldSuppress(Exception ex) => false;
}

internal sealed class TestLoggerProvider : ILoggerProvider
{
  private readonly List<LogEntry> _entries = new();

  public IReadOnlyList<LogEntry> Entries => _entries;

  public ILogger CreateLogger(string categoryName)
  {
    return new TestLogger(categoryName, _entries);
  }

  public void Dispose()
  {
  }

  internal sealed record LogEntry(string Category, LogLevel Level, EventId EventId, string Message, Exception? Exception);

  private sealed class TestLogger : ILogger
  {
    private readonly string _category;
    private readonly List<LogEntry> _entries;

    public TestLogger(string category, List<LogEntry> entries)
    {
      _category = category;
      _entries = entries;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
      return new NoopDisposable();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
      _entries.Add(new LogEntry(_category, logLevel, eventId, formatter(state, exception), exception));
    }

    private sealed class NoopDisposable : IDisposable
    {
      public void Dispose()
      {
      }
    }
  }
}

internal sealed class TestHubCallerContext : HubCallerContext
{
  private readonly FeatureCollection _features = new();
  private readonly Dictionary<object, object?> _items = new();
  private readonly ClaimsPrincipal _user = new(new ClaimsIdentity());

  public override string ConnectionId => "conn-1";
  public override string? UserIdentifier => "user-1";
  public override ClaimsPrincipal? User => _user;
  public override IDictionary<object, object?> Items => _items;
  public override IFeatureCollection Features => _features;
  public override CancellationToken ConnectionAborted => CancellationToken.None;

  public override void Abort()
  {
  }
}

internal sealed class TestHub : Hub
{
  public string Echo(string value) => value;
}
