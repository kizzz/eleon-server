using Eleon.Logging.Lib.VportalLogger;
using Eleon.Logging.Lib.VportalLogging;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace Eleon.Logging.Lib.Tests;

public class VportalLoggerCompatTests
{
  [Fact]
  public void CaptureAndSuppress_reports_exception_with_context()
  {
    var reporter = new RecordingExceptionReporter();
    var tenant = Substitute.For<ICurrentTenant>();
    tenant.Id.Returns(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

    var logger = new VPortalLogger<VportalLoggerCompatTests>(
        NullLogger<VportalLoggerCompatTests>.Instance,
        reporter,
        tenant);

    var exception = new InvalidOperationException("boom");

    logger.CaptureAndSuppress(exception, "TestOperation");

    reporter.CallCount.Should().Be(1);
    reporter.LastException.Should().Be(exception);
    reporter.LastContext.Should().NotBeNull();
    reporter.LastContext![VportalLogProperties.Component].Should().Be(nameof(VportalLoggerCompatTests));
    reporter.LastContext[VportalLogProperties.Operation].Should().Be("TestOperation");
    reporter.LastContext[VportalLogProperties.Tenant].Should().Be("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
  }

  [Fact]
  public void Capture_rethrows_after_reporting()
  {
    var reporter = new RecordingExceptionReporter();
    var logger = new VPortalLogger<VportalLoggerCompatTests>(
        NullLogger<VportalLoggerCompatTests>.Instance,
        reporter,
        currentTenant: null);

    var exception = new InvalidOperationException("boom");

    Action action = () => logger.Capture(exception, "TestOperation");

    action.Should().Throw<InvalidOperationException>();
    reporter.CallCount.Should().Be(1);
  }
}
