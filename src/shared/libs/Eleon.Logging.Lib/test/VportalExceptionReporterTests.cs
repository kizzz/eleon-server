using Eleon.Logging.Lib.VportalLogging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sentry;
using Xunit;

namespace Eleon.Logging.Lib.Tests;

public class VportalExceptionReporterTests
{
  [Fact]
  public void ShouldSuppress_returns_true_when_predicate_matches()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Unexpected);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var options = Options.Create(new VportalExceptionOptions
    {
      SuppressPredicate = _ => true
    });

    var reporter = new VportalExceptionReporter(classifier, logger, options, hub: null);

    reporter.ShouldSuppress(new InvalidOperationException()).Should().BeTrue();
  }

  [Fact]
  public void Report_captures_exception_and_sets_tags_and_extras()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Unexpected);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var options = Options.Create(new VportalExceptionOptions
    {
      PreferRouteTemplateOverPath = true
    });

    var hub = Substitute.For<IHub>();
    Action<Scope>? capturedScope = null;

    hub.CaptureEvent(Arg.Any<SentryEvent>(), Arg.Any<Action<Scope>>())
      .Returns(callInfo =>
      {
        capturedScope = callInfo.Arg<Action<Scope>>();
        return SentryId.Empty;
      });

    var reporter = new VportalExceptionReporter(classifier, logger, options, hub);

    var context = new Dictionary<string, object?>
    {
      [VportalLogProperties.Tenant] = "tenant-1",
      [VportalLogProperties.TraceId] = "trace-1",
      [VportalLogProperties.Route] = "/api/items/{id}",
      [VportalLogProperties.Path] = "/api/items/123",
      ["Custom"] = 42
    };

    reporter.Report(new InvalidOperationException("boom"), context);

    capturedScope.Should().NotBeNull();

    var scope = new Scope(new SentryOptions());
    capturedScope!(scope);

    scope.Tags[VportalLogProperties.Tenant].Should().Be("tenant-1");
    scope.Tags[VportalLogProperties.TraceId].Should().Be("trace-1");
    scope.Tags[VportalLogProperties.ExceptionKind].Should().Be(ExceptionKind.Unexpected.ToString());
    scope.Tags[VportalLogProperties.Route].Should().Be("/api/items/{id}");
    scope.Extra["Custom"].Should().Be(42);
  }

  [Fact]
  public void Report_uses_path_when_route_preference_disabled()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Unexpected);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var options = Options.Create(new VportalExceptionOptions
    {
      PreferRouteTemplateOverPath = false
    });

    var hub = Substitute.For<IHub>();
    Action<Scope>? capturedScope = null;

    hub.CaptureEvent(Arg.Any<SentryEvent>(), Arg.Any<Action<Scope>>())
      .Returns(callInfo =>
      {
        capturedScope = callInfo.Arg<Action<Scope>>();
        return SentryId.Empty;
      });

    var reporter = new VportalExceptionReporter(classifier, logger, options, hub);

    var context = new Dictionary<string, object?>
    {
      [VportalLogProperties.Route] = "/api/items/{id}",
      [VportalLogProperties.Path] = "/api/items/123"
    };

    reporter.Report(new InvalidOperationException("boom"), context);

    capturedScope.Should().NotBeNull();
    var scope = new Scope(new SentryOptions());
    capturedScope!(scope);

    scope.Tags[VportalLogProperties.Route].Should().Be("/api/items/123");
  }

  [Fact]
  public void Report_suppressed_business_exception_does_not_capture()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Business);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var options = Options.Create(new VportalExceptionOptions
    {
      CaptureBusinessExceptionsToSentry = false
    });

    var hub = Substitute.For<IHub>();

    var reporter = new VportalExceptionReporter(classifier, logger, options, hub);

    reporter.Report(new InvalidOperationException("boom"), new Dictionary<string, object?>());

    hub.DidNotReceiveWithAnyArgs().CaptureEvent(default!, default!);
    loggerProvider.Entries.Should().Contain(e => e.Level == LogLevel.Information);
  }

  [Fact]
  public void Report_suppressed_cancellation_exception_does_not_capture()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Cancellation);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder =>
    {
      builder.SetMinimumLevel(LogLevel.Debug);
      builder.AddProvider(loggerProvider);
    });
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var options = Options.Create(new VportalExceptionOptions
    {
      CaptureCancellationToSentry = false
    });

    var hub = Substitute.For<IHub>();
    var reporter = new VportalExceptionReporter(classifier, logger, options, hub);

    reporter.Report(new OperationCanceledException("cancel"), new Dictionary<string, object?>());

    hub.DidNotReceiveWithAnyArgs().CaptureEvent(default!, default!);
    loggerProvider.Entries.Should().Contain(e => e.Level == LogLevel.Debug);
  }

  [Fact]
  public void Report_captures_business_exception_when_enabled()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Business);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var options = Options.Create(new VportalExceptionOptions
    {
      CaptureBusinessExceptionsToSentry = true
    });

    var hub = Substitute.For<IHub>();
    Action<Scope>? capturedScope = null;

    hub.CaptureEvent(Arg.Any<SentryEvent>(), Arg.Any<Action<Scope>>())
      .Returns(callInfo =>
      {
        capturedScope = callInfo.Arg<Action<Scope>>();
        return SentryId.Empty;
      });

    var reporter = new VportalExceptionReporter(classifier, logger, options, hub);

    reporter.Report(new InvalidOperationException("boom"), new Dictionary<string, object?>());

    capturedScope.Should().NotBeNull();
    loggerProvider.Entries.Should().Contain(e => e.Level == LogLevel.Information);
  }

  [Fact]
  public void Report_captures_cancellation_exception_when_enabled()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Cancellation);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var options = Options.Create(new VportalExceptionOptions
    {
      CaptureCancellationToSentry = true
    });

    var hub = Substitute.For<IHub>();
    Action<Scope>? capturedScope = null;

    hub.CaptureEvent(Arg.Any<SentryEvent>(), Arg.Any<Action<Scope>>())
      .Returns(callInfo =>
      {
        capturedScope = callInfo.Arg<Action<Scope>>();
        return SentryId.Empty;
      });

    var reporter = new VportalExceptionReporter(classifier, logger, options, hub);

    reporter.Report(new OperationCanceledException("cancel"), new Dictionary<string, object?>());

    capturedScope.Should().NotBeNull();
  }

  [Fact]
  public void Report_does_not_add_standard_tags_as_extras()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Unexpected);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var options = Options.Create(new VportalExceptionOptions());

    var hub = Substitute.For<IHub>();
    Action<Scope>? capturedScope = null;

    hub.CaptureEvent(Arg.Any<SentryEvent>(), Arg.Any<Action<Scope>>())
      .Returns(callInfo =>
      {
        capturedScope = callInfo.Arg<Action<Scope>>();
        return SentryId.Empty;
      });

    var reporter = new VportalExceptionReporter(classifier, logger, options, hub);

    var context = new Dictionary<string, object?>
    {
      [VportalLogProperties.Tenant] = "tenant-1",
      [VportalLogProperties.TraceId] = "trace-1",
      [VportalLogProperties.ExceptionKind] = "Unexpected",
      [VportalLogProperties.Route] = "/api/items/{id}",
      ["Custom"] = "value"
    };

    reporter.Report(new InvalidOperationException("boom"), context);

    capturedScope.Should().NotBeNull();
    var scope = new Scope(new SentryOptions());
    capturedScope!(scope);

    scope.Extra.ContainsKey(VportalLogProperties.Tenant).Should().BeFalse();
    scope.Extra.ContainsKey(VportalLogProperties.TraceId).Should().BeFalse();
    scope.Extra.ContainsKey(VportalLogProperties.ExceptionKind).Should().BeFalse();
    scope.Extra.ContainsKey(VportalLogProperties.Route).Should().BeFalse();
    scope.Extra["Custom"].Should().Be("value");
  }

  [Fact]
  public void Report_uses_activity_trace_when_context_missing_trace_id()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Unexpected);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var options = Options.Create(new VportalExceptionOptions());

    var hub = Substitute.For<IHub>();
    Action<Scope>? capturedScope = null;

    hub.CaptureEvent(Arg.Any<SentryEvent>(), Arg.Any<Action<Scope>>())
      .Returns(callInfo =>
      {
        capturedScope = callInfo.Arg<Action<Scope>>();
        return SentryId.Empty;
      });

    var reporter = new VportalExceptionReporter(classifier, logger, options, hub);

    using var activity = new System.Diagnostics.Activity("test");
    activity.Start();

    var context = new Dictionary<string, object?>
    {
      [VportalLogProperties.Tenant] = "tenant-2"
    };

    reporter.Report(new InvalidOperationException("boom"), context);

    capturedScope.Should().NotBeNull();
    var scope = new Scope(new SentryOptions());
    capturedScope!(scope);

    scope.Tags[VportalLogProperties.TraceId].Should().Be(activity.TraceId.ToString());
  }

  [Fact]
  public void Report_uses_host_and_unknown_trace_when_context_missing()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Unexpected);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var options = Options.Create(new VportalExceptionOptions());

    var hub = Substitute.For<IHub>();
    Action<Scope>? capturedScope = null;

    hub.CaptureEvent(Arg.Any<SentryEvent>(), Arg.Any<Action<Scope>>())
      .Returns(callInfo =>
      {
        capturedScope = callInfo.Arg<Action<Scope>>();
        return SentryId.Empty;
      });

    var reporter = new VportalExceptionReporter(classifier, logger, options, hub);

    reporter.Report(new InvalidOperationException("boom"), null);

    capturedScope.Should().NotBeNull();
    var scope = new Scope(new SentryOptions());
    capturedScope!(scope);

    scope.Tags[VportalLogProperties.Tenant].Should().Be("Host");
    scope.Tags[VportalLogProperties.TraceId].Should().Be("Unknown");
  }

  [Fact]
  public void Report_does_not_throw_when_hub_is_null()
  {
    var classifier = Substitute.For<IExceptionClassifier>();
    classifier.Classify(Arg.Any<Exception>()).Returns(ExceptionKind.Unexpected);

    var loggerProvider = new TestLoggerProvider();
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
    var logger = loggerFactory.CreateLogger<VportalExceptionReporter>();

    var reporter = new VportalExceptionReporter(classifier, logger, Options.Create(new VportalExceptionOptions()), hub: null);

    reporter.Report(new InvalidOperationException("boom"), new Dictionary<string, object?>());

    loggerProvider.Entries.Should().Contain(e => e.Level == LogLevel.Error);
  }
}
