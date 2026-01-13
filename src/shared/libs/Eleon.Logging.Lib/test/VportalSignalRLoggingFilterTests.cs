using Eleon.Logging.Lib.VportalLogging;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Diagnostics;
using System.Reflection;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace Eleon.Logging.Lib.Tests;

public class VportalSignalRLoggingFilterTests
{
  [Fact]
  public async Task InvokeMethodAsync_creates_scope_and_returns_result()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var hostOptions = Options.Create(new VportalLoggingHostOptions { EnableSignalRLogging = true });

    var tenant = Substitute.For<ICurrentTenant>();
    tenant.Id.Returns(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

    var filter = new VportalSignalRLoggingFilter(
        scopeFactory,
        reporter,
        NullLogger<VportalSignalRLoggingFilter>.Instance,
        hostOptions,
        tenant);

    var hub = new TestHub();
    var callerContext = new TestHubCallerContext();
    var serviceProvider = new ServiceCollection().BuildServiceProvider();
    var method = typeof(TestHub).GetMethod(nameof(TestHub.Echo), BindingFlags.Instance | BindingFlags.Public)!;

    var invocationContext = new HubInvocationContext(callerContext, serviceProvider, hub, method, new object?[] { "hi" });

    var result = await filter.InvokeMethodAsync(invocationContext, _ => new ValueTask<object?>("ok"));

    result.Should().Be("ok");
    scopeFactory.LastOperationName.Should().Be("SignalR TestHub.Echo");
    scopeFactory.LastContext![VportalLogProperties.Component].Should().Be("TestHub");
    scopeFactory.LastContext[VportalLogProperties.Operation].Should().Be("Echo");
    scopeFactory.LastContext[VportalLogProperties.Tenant].Should().Be("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
  }

  [Fact]
  public async Task InvokeMethodAsync_reports_and_rethrows_exceptions()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var hostOptions = Options.Create(new VportalLoggingHostOptions { EnableSignalRLogging = true });

    var filter = new VportalSignalRLoggingFilter(
        scopeFactory,
        reporter,
        NullLogger<VportalSignalRLoggingFilter>.Instance,
        hostOptions,
        currentTenant: null);

    var hub = new TestHub();
    var callerContext = new TestHubCallerContext();
    var serviceProvider = new ServiceCollection().BuildServiceProvider();
    var method = typeof(TestHub).GetMethod(nameof(TestHub.Echo), BindingFlags.Instance | BindingFlags.Public)!;

    var invocationContext = new HubInvocationContext(callerContext, serviceProvider, hub, method, new object?[] { "hi" });

    await Assert.ThrowsAsync<InvalidOperationException>(async () =>
    {
      await filter.InvokeMethodAsync(invocationContext, _ => throw new InvalidOperationException("boom"));
    });

    reporter.CallCount.Should().Be(1);
    reporter.LastException.Should().BeOfType<InvalidOperationException>();
  }

  [Fact]
  public async Task InvokeMethodAsync_includes_trace_and_span_when_activity_present()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var hostOptions = Options.Create(new VportalLoggingHostOptions { EnableSignalRLogging = true });

    var filter = new VportalSignalRLoggingFilter(
        scopeFactory,
        reporter,
        NullLogger<VportalSignalRLoggingFilter>.Instance,
        hostOptions,
        currentTenant: null);

    var hub = new TestHub();
    var callerContext = new TestHubCallerContext();
    var serviceProvider = new ServiceCollection().BuildServiceProvider();
    var method = typeof(TestHub).GetMethod(nameof(TestHub.Echo), BindingFlags.Instance | BindingFlags.Public)!;

    var invocationContext = new HubInvocationContext(callerContext, serviceProvider, hub, method, new object?[] { "hi" });

    using var activity = new Activity("signalr");
    activity.Start();

    await filter.InvokeMethodAsync(invocationContext, _ => new ValueTask<object?>("ok"));

    scopeFactory.LastContext.Should().NotBeNull();
    scopeFactory.LastContext![VportalLogProperties.TraceId].Should().Be(activity.TraceId.ToString());
    scopeFactory.LastContext[VportalLogProperties.SpanId].Should().Be(activity.SpanId.ToString());
  }

  [Fact]
  public async Task OnConnectedAsync_creates_scope_and_calls_next()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var hostOptions = Options.Create(new VportalLoggingHostOptions { EnableSignalRLogging = true });

    var filter = new VportalSignalRLoggingFilter(
        scopeFactory,
        reporter,
        NullLogger<VportalSignalRLoggingFilter>.Instance,
        hostOptions,
        currentTenant: null);

    var hub = new TestHub();
    var callerContext = new TestHubCallerContext();
    var serviceProvider = new ServiceCollection().BuildServiceProvider();
    var lifetimeContext = new HubLifetimeContext(callerContext, serviceProvider, hub);

    var called = false;

    await filter.OnConnectedAsync(lifetimeContext, _ =>
    {
      called = true;
      return Task.CompletedTask;
    });

    called.Should().BeTrue();
    scopeFactory.LastOperationName.Should().Be("SignalR TestHub.Connected");
    scopeFactory.LastContext![VportalLogProperties.Tenant].Should().Be("Host");
  }

  [Fact]
  public async Task OnDisconnectedAsync_reports_exception_when_present()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var hostOptions = Options.Create(new VportalLoggingHostOptions { EnableSignalRLogging = true });

    var filter = new VportalSignalRLoggingFilter(
        scopeFactory,
        reporter,
        NullLogger<VportalSignalRLoggingFilter>.Instance,
        hostOptions,
        currentTenant: null);

    var hub = new TestHub();
    var callerContext = new TestHubCallerContext();
    var serviceProvider = new ServiceCollection().BuildServiceProvider();
    var lifetimeContext = new HubLifetimeContext(callerContext, serviceProvider, hub);

    await filter.OnDisconnectedAsync(lifetimeContext, new InvalidOperationException("boom"), (_, _) => Task.CompletedTask);

    reporter.CallCount.Should().Be(1);
    reporter.LastException.Should().BeOfType<InvalidOperationException>();
    scopeFactory.LastOperationName.Should().Be("SignalR TestHub.Disconnected");
  }

  [Fact]
  public async Task InvokeMethodAsync_skips_when_disabled()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var hostOptions = Options.Create(new VportalLoggingHostOptions { EnableSignalRLogging = false });

    var filter = new VportalSignalRLoggingFilter(
        scopeFactory,
        reporter,
        NullLogger<VportalSignalRLoggingFilter>.Instance,
        hostOptions,
        currentTenant: null);

    var hub = new TestHub();
    var callerContext = new TestHubCallerContext();
    var serviceProvider = new ServiceCollection().BuildServiceProvider();
    var method = typeof(TestHub).GetMethod(nameof(TestHub.Echo), BindingFlags.Instance | BindingFlags.Public)!;

    var invocationContext = new HubInvocationContext(callerContext, serviceProvider, hub, method, new object?[] { "hi" });

    var result = await filter.InvokeMethodAsync(invocationContext, _ => new ValueTask<object?>("ok"));

    result.Should().Be("ok");
    scopeFactory.LastOperationName.Should().BeNull();
    reporter.CallCount.Should().Be(0);
  }

  [Fact]
  public async Task OnConnectedAsync_skips_when_disabled()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var hostOptions = Options.Create(new VportalLoggingHostOptions { EnableSignalRLogging = false });

    var filter = new VportalSignalRLoggingFilter(
        scopeFactory,
        reporter,
        NullLogger<VportalSignalRLoggingFilter>.Instance,
        hostOptions,
        currentTenant: null);

    var hub = new TestHub();
    var callerContext = new TestHubCallerContext();
    var serviceProvider = new ServiceCollection().BuildServiceProvider();
    var lifetimeContext = new HubLifetimeContext(callerContext, serviceProvider, hub);

    await filter.OnConnectedAsync(lifetimeContext, _ => Task.CompletedTask);

    scopeFactory.LastOperationName.Should().BeNull();
    reporter.CallCount.Should().Be(0);
  }

  [Fact]
  public async Task OnDisconnectedAsync_does_not_report_when_exception_null()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var hostOptions = Options.Create(new VportalLoggingHostOptions { EnableSignalRLogging = true });

    var filter = new VportalSignalRLoggingFilter(
        scopeFactory,
        reporter,
        NullLogger<VportalSignalRLoggingFilter>.Instance,
        hostOptions,
        currentTenant: null);

    var hub = new TestHub();
    var callerContext = new TestHubCallerContext();
    var serviceProvider = new ServiceCollection().BuildServiceProvider();
    var lifetimeContext = new HubLifetimeContext(callerContext, serviceProvider, hub);

    await filter.OnDisconnectedAsync(lifetimeContext, null, (_, _) => Task.CompletedTask);

    reporter.CallCount.Should().Be(0);
    scopeFactory.LastOperationName.Should().Be("SignalR TestHub.Disconnected");
  }

  [Fact]
  public async Task OnDisconnectedAsync_skips_when_disabled()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var hostOptions = Options.Create(new VportalLoggingHostOptions { EnableSignalRLogging = false });

    var filter = new VportalSignalRLoggingFilter(
        scopeFactory,
        reporter,
        NullLogger<VportalSignalRLoggingFilter>.Instance,
        hostOptions,
        currentTenant: null);

    var hub = new TestHub();
    var callerContext = new TestHubCallerContext();
    var serviceProvider = new ServiceCollection().BuildServiceProvider();
    var lifetimeContext = new HubLifetimeContext(callerContext, serviceProvider, hub);

    await filter.OnDisconnectedAsync(lifetimeContext, new InvalidOperationException("boom"), (_, _) => Task.CompletedTask);

    reporter.CallCount.Should().Be(0);
    scopeFactory.LastOperationName.Should().BeNull();
  }
}
