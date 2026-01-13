using Eleon.Logging.Lib.VportalLogging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Diagnostics;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace Eleon.Logging.Lib.Tests;

public class VportalRequestLoggingMiddlewareTests
{
  [Fact]
  public async Task InvokeAsync_skips_excluded_paths()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var options = Options.Create(new VportalRequestLoggingOptions
    {
      ExcludedPathPrefixes = new List<string> { "/health" }
    });

    var middleware = new VportalRequestLoggingMiddleware(
        _ => Task.CompletedTask,
        NullLogger<VportalRequestLoggingMiddleware>.Instance,
        scopeFactory,
        reporter,
        options);

    var context = new DefaultHttpContext();
    context.Request.Path = "/health/check";

    await middleware.InvokeAsync(context);

    scopeFactory.LastContext.Should().BeNull();
    reporter.CallCount.Should().Be(0);
  }

  [Fact]
  public async Task InvokeAsync_excluded_paths_are_case_insensitive()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var options = Options.Create(new VportalRequestLoggingOptions
    {
      ExcludedPathPrefixes = new List<string> { "/Health" }
    });

    var middleware = new VportalRequestLoggingMiddleware(
        _ => Task.CompletedTask,
        NullLogger<VportalRequestLoggingMiddleware>.Instance,
        scopeFactory,
        reporter,
        options);

    var context = new DefaultHttpContext();
    context.Request.Path = "/health/check";

    await middleware.InvokeAsync(context);

    scopeFactory.LastContext.Should().BeNull();
    reporter.CallCount.Should().Be(0);
  }

  [Fact]
  public async Task InvokeAsync_prefers_route_template_over_path()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var options = Options.Create(new VportalRequestLoggingOptions
    {
      PreferRouteTemplateOverPath = true
    });

    var middleware = new VportalRequestLoggingMiddleware(
        _ => Task.CompletedTask,
        NullLogger<VportalRequestLoggingMiddleware>.Instance,
        scopeFactory,
        reporter,
        options);

    var context = new DefaultHttpContext();
    context.Request.Method = "GET";
    context.Request.Path = "/api/items/123";

    var endpoint = new RouteEndpoint(
        _ => Task.CompletedTask,
        RoutePatternFactory.Parse("api/items/{id}"),
        0,
        EndpointMetadataCollection.Empty,
        "test");

    context.SetEndpoint(endpoint);

    using var activity = new Activity("test");
    activity.Start();

    var tenant = Substitute.For<ICurrentTenant>();
    tenant.Id.Returns(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

    await middleware.InvokeAsync(context, tenant);

    scopeFactory.LastOperationName.Should().Be("GET api/items/{id}");
    scopeFactory.LastContext.Should().NotBeNull();
    scopeFactory.LastContext![VportalLogProperties.Route].Should().Be("api/items/{id}");
    scopeFactory.LastContext.ContainsKey(VportalLogProperties.Path).Should().BeFalse();
    scopeFactory.LastContext[VportalLogProperties.Tenant].Should().Be("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    scopeFactory.LastContext[VportalLogProperties.TraceId].Should().NotBeNull();
  }

  [Fact]
  public async Task InvokeAsync_uses_path_when_route_preference_disabled()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var options = Options.Create(new VportalRequestLoggingOptions
    {
      PreferRouteTemplateOverPath = false
    });

    var middleware = new VportalRequestLoggingMiddleware(
        _ => Task.CompletedTask,
        NullLogger<VportalRequestLoggingMiddleware>.Instance,
        scopeFactory,
        reporter,
        options);

    var context = new DefaultHttpContext();
    context.Request.Method = "GET";
    context.Request.Path = "/api/items/123";

    var endpoint = new RouteEndpoint(
        _ => Task.CompletedTask,
        RoutePatternFactory.Parse("api/items/{id}"),
        0,
        EndpointMetadataCollection.Empty,
        "test");

    context.SetEndpoint(endpoint);

    await middleware.InvokeAsync(context);

    scopeFactory.LastOperationName.Should().Be("GET api/items/{id}");
    scopeFactory.LastContext!.ContainsKey(VportalLogProperties.Path).Should().BeTrue();
    scopeFactory.LastContext[VportalLogProperties.Path].Should().Be("/api/items/123");
  }

  [Fact]
  public async Task InvokeAsync_reports_and_rethrows_exceptions()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var options = Options.Create(new VportalRequestLoggingOptions());

    var middleware = new VportalRequestLoggingMiddleware(
        _ => throw new InvalidOperationException("boom"),
        NullLogger<VportalRequestLoggingMiddleware>.Instance,
        scopeFactory,
        reporter,
        options);

    var context = new DefaultHttpContext();
    context.Request.Method = "POST";
    context.Request.Path = "/api/test";

    await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

    reporter.CallCount.Should().Be(1);
    reporter.LastException.Should().BeOfType<InvalidOperationException>();
  }

  [Fact]
  public async Task InvokeAsync_skips_when_disabled()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var options = Options.Create(new VportalRequestLoggingOptions
    {
      Enable = false
    });

    var called = false;
    var middleware = new VportalRequestLoggingMiddleware(
        _ =>
        {
          called = true;
          return Task.CompletedTask;
        },
        NullLogger<VportalRequestLoggingMiddleware>.Instance,
        scopeFactory,
        reporter,
        options);

    var context = new DefaultHttpContext();
    context.Request.Path = "/api/test";

    await middleware.InvokeAsync(context);

    called.Should().BeTrue();
    scopeFactory.LastContext.Should().BeNull();
    reporter.CallCount.Should().Be(0);
  }

  [Fact]
  public async Task InvokeAsync_uses_path_and_trace_identifier_when_no_activity_and_no_route()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var options = Options.Create(new VportalRequestLoggingOptions());

    var middleware = new VportalRequestLoggingMiddleware(
        _ => Task.CompletedTask,
        NullLogger<VportalRequestLoggingMiddleware>.Instance,
        scopeFactory,
        reporter,
        options);

    var context = new DefaultHttpContext();
    context.TraceIdentifier = "trace-xyz";
    context.Request.Method = "GET";
    context.Request.Path = "/api/items/123";

    await middleware.InvokeAsync(context);

    scopeFactory.LastOperationName.Should().Be("GET /api/items/123");
    scopeFactory.LastContext.Should().NotBeNull();
    scopeFactory.LastContext![VportalLogProperties.Path].Should().Be("/api/items/123");
    scopeFactory.LastContext[VportalLogProperties.TraceId].Should().Be("trace-xyz");
    scopeFactory.LastContext[VportalLogProperties.Tenant].Should().Be("Host");
  }

  [Fact]
  public async Task InvokeAsync_sets_span_id_when_activity_present()
  {
    var scopeFactory = new RecordingOperationScopeFactory();
    var reporter = new RecordingExceptionReporter();
    var options = Options.Create(new VportalRequestLoggingOptions());

    var middleware = new VportalRequestLoggingMiddleware(
        _ => Task.CompletedTask,
        NullLogger<VportalRequestLoggingMiddleware>.Instance,
        scopeFactory,
        reporter,
        options);

    var context = new DefaultHttpContext();
    context.Request.Method = "GET";
    context.Request.Path = "/api/items/123";

    using var activity = new Activity("test");
    activity.Start();

    await middleware.InvokeAsync(context);

    scopeFactory.LastContext.Should().NotBeNull();
    scopeFactory.LastContext![VportalLogProperties.SpanId].Should().Be(activity.SpanId.ToString());
  }
}
