using Eleon.Logging.Lib.VportalLogging;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Eleon.Logging.Lib.Tests;

public class BoundaryLoggerTests
{
  [Fact]
  public void BeginConsumer_includes_message_context()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var boundaryLogger = new BoundaryLogger(scopeFactory, NullLogger<BoundaryLogger>.Instance);

    var message = new TestMessage
    {
      Id = "msg-1",
      CorrelationId = "corr-1",
      TenantId = "tenant-1"
    };

    using var scope = boundaryLogger.BeginConsumer(message);

    scope.Context[VportalLogProperties.MessageType].Should().Be(nameof(TestMessage));
    scope.Context[VportalLogProperties.MessageId].Should().Be("msg-1");
    scope.Context[VportalLogProperties.CorrelationId].Should().Be("corr-1");
    scope.Context[VportalLogProperties.Tenant].Should().Be("tenant-1");
  }

  [Fact]
  public void BeginConsumer_uses_job_context_when_present()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var boundaryLogger = new BoundaryLogger(scopeFactory, NullLogger<BoundaryLogger>.Instance);

    var message = new JobEnvelope
    {
      TenantId = "tenant-2",
      ExecutionId = "exec-1",
      BackgroundJob = new JobInfo { Id = "job-1", Type = "DemoJob" }
    };

    using var scope = boundaryLogger.BeginConsumer(message);

    scope.Context[VportalLogProperties.JobName].Should().Be("DemoJob");
    scope.Context[VportalLogProperties.JobId].Should().Be("job-1");
    scope.Context[VportalLogProperties.CorrelationId].Should().Be("exec-1");
    scope.Context[VportalLogProperties.Tenant].Should().Be("tenant-2");
  }

  private sealed class TestMessage
  {
    public string? Id { get; set; }
    public string? CorrelationId { get; set; }
    public string? TenantId { get; set; }
  }

  private sealed class JobEnvelope
  {
    public string? TenantId { get; set; }
    public string? ExecutionId { get; set; }
    public JobInfo? BackgroundJob { get; set; }
  }

  private sealed class JobInfo
  {
    public string? Id { get; set; }
    public string? Type { get; set; }
  }
}
