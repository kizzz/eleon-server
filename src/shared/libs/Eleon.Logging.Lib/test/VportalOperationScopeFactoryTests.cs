using Eleon.Logging.Lib.VportalLogging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Eleon.Logging.Lib.Tests;

public class VportalOperationScopeFactoryTests
{
  [Fact]
  public void Begin_sets_operation_and_merges_context()
  {
    var logger = new RecordingLogger();
    var factory = new VportalOperationScopeFactory(logger);

    var context = new Dictionary<string, object?>
    {
      [VportalLogProperties.Operation] = "Override",
      [VportalLogProperties.Tenant] = "tenant-1"
    };

    using var _ = factory.Begin("MyOperation", context);

    var scopeState = logger.LastScope as IReadOnlyDictionary<string, object?>;
    scopeState.Should().NotBeNull();
    scopeState![VportalLogProperties.Operation].Should().Be("MyOperation");
    scopeState[VportalLogProperties.Tenant].Should().Be("tenant-1");
  }

  [Fact]
  public void Begin_sets_operation_when_context_null()
  {
    var logger = new RecordingLogger();
    var factory = new VportalOperationScopeFactory(logger);

    using var _ = factory.Begin("MyOperation");

    var scopeState = logger.LastScope as IReadOnlyDictionary<string, object?>;
    scopeState.Should().NotBeNull();
    scopeState![VportalLogProperties.Operation].Should().Be("MyOperation");
  }

  private sealed class RecordingLogger : ILogger<VportalOperationScopeFactory>
  {
    public object? LastScope { get; private set; }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
      LastScope = state;
      return new NoopDisposable();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }

    private sealed class NoopDisposable : IDisposable
    {
      public void Dispose()
      {
      }
    }
  }
}
