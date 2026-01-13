using System.Diagnostics;
using System.Net.Mime;
using FluentAssertions;
using Logging.Module.ErrorHandling.Enrichers;
using Logging.Module.ErrorHandling.Mappers;
using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helpers for testing error handling scenarios including HTTP context, mappers, enrichers, and ProblemDetails.
/// </summary>
public static class ErrorHandlingTestHelpers
{
    /// <summary>
    /// Creates a mock HttpContext with default settings.
    /// </summary>
    public static HttpContext CreateHttpContext(
        string? acceptHeader = null,
        string? path = "/test",
        string? method = "GET",
        Dictionary<string, string>? headers = null,
        Dictionary<string, object?>? items = null)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path ?? "/test";
        context.Request.Method = method ?? "GET";
        context.TraceIdentifier = Guid.NewGuid().ToString();

        if (!string.IsNullOrEmpty(acceptHeader))
        {
            context.Request.Headers.Accept = acceptHeader;
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                context.Request.Headers[header.Key] = header.Value;
            }
        }

        if (items != null)
        {
            foreach (var item in items)
            {
                context.Items[item.Key] = item.Value;
            }
        }

        return context;
    }

    /// <summary>
    /// Creates a mock HttpContext configured for HTML requests (browser).
    /// </summary>
    public static HttpContext CreateHtmlRequestContext(string? path = "/test")
    {
        return CreateHttpContext(acceptHeader: MediaTypeNames.Text.Html, path: path);
    }

    /// <summary>
    /// Creates a mock HttpContext configured for JSON requests (API).
    /// </summary>
    public static HttpContext CreateJsonRequestContext(string? path = "/test")
    {
        return CreateHttpContext(acceptHeader: "application/json", path: path);
    }

    /// <summary>
    /// Creates a mock HttpContext with sensitive headers (for testing redaction).
    /// </summary>
    public static HttpContext CreateContextWithSensitiveHeaders(string? path = "/test")
    {
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer secret-token" },
            { "Cookie", "session=abc123" },
            { "X-Api-Key", "api-key-123" },
            { "User-Agent", "TestAgent/1.0" },
            { "Accept", "application/json" }
        };
        return CreateHttpContext(headers: headers, path: path);
    }

    /// <summary>
    /// Creates a mock IExceptionMapper with custom mapping logic.
    /// </summary>
    public static IExceptionMapper CreateMockExceptionMapper(
        Func<Exception, (int HttpStatus, string AppCode, string Title)>? mapFunc = null)
    {
        var mapper = Substitute.For<IExceptionMapper>();
        if (mapFunc != null)
        {
            mapper.Map(Arg.Any<Exception>()).Returns(c => mapFunc(c.Arg<Exception>()));
        }
        else
        {
            mapper.Map(Arg.Any<Exception>()).Returns((500, "ELEON-0000", "Internal Server Error"));
        }
        return mapper;
    }

    /// <summary>
    /// Creates a mock IErrorEnricher with custom enrichment logic.
    /// </summary>
    public static IErrorEnricher CreateMockErrorEnricher(Action<ProblemDetails, HttpContext, Exception?>? enrichAction = null)
    {
        var enricher = Substitute.For<IErrorEnricher>();
        if (enrichAction != null)
        {
            enricher.When(x => x.Enrich(Arg.Any<ProblemDetails>(), Arg.Any<HttpContext>(), Arg.Any<Exception?>()))
                .Do(c => enrichAction(c.Arg<ProblemDetails>(), c.Arg<HttpContext>(), c.Arg<Exception?>()));
        }
        return enricher;
    }

    /// <summary>
    /// Creates a mock IProblemDetailsService.
    /// </summary>
    public static IProblemDetailsService CreateMockProblemDetailsService(bool returnsTrue = true)
    {
        var service = Substitute.For<IProblemDetailsService>();
        service.TryWriteAsync(Arg.Any<ProblemDetailsContext>())
            .Returns(new ValueTask<bool>(returnsTrue));
        return service;
    }

    /// <summary>
    /// Creates a mock IHostEnvironment.
    /// </summary>
    public static IHostEnvironment CreateMockHostEnvironment(bool isDevelopment = false, string environmentName = "Production")
    {
        var env = Substitute.For<IHostEnvironment>();
        env.IsDevelopment().Returns(isDevelopment);
        env.EnvironmentName.Returns(environmentName);
        return env;
    }

    /// <summary>
    /// Creates ErrorHandlingOptions with custom configuration.
    /// </summary>
    public static ErrorHandlingOptions CreateErrorHandlingOptions(Action<ErrorHandlingOptions>? configure = null)
    {
        var options = new ErrorHandlingOptions();
        configure?.Invoke(options);
        return options;
    }

    /// <summary>
    /// Creates an exception with specific data entries.
    /// </summary>
    public static Exception CreateExceptionWithData(
        string message = "Test exception",
        Dictionary<string, object?>? data = null,
        Exception? innerException = null)
    {
        var exception = new Exception(message, innerException);
        if (data != null)
        {
            foreach (var kvp in data)
            {
                exception.Data[kvp.Key] = kvp.Value;
            }
        }
        return exception;
    }

    /// <summary>
    /// Creates an exception with a StatusCode in its Data.
    /// </summary>
    public static Exception CreateExceptionWithStatusCode(int statusCode, string message = "Test exception")
    {
        return CreateExceptionWithData(message, new Dictionary<string, object?> { { "StatusCode", statusCode } });
    }

    /// <summary>
    /// Creates an exception with a FriendlyMessage in its Data.
    /// </summary>
    public static Exception CreateExceptionWithFriendlyMessage(string friendlyMessage, string message = "Test exception")
    {
        return CreateExceptionWithData(message, new Dictionary<string, object?> { { "FriendlyMessage", friendlyMessage } });
    }

    /// <summary>
    /// Creates a deep exception chain (for testing depth limits).
    /// </summary>
    public static Exception CreateDeepExceptionChain(int depth, string baseMessage = "Level")
    {
        Exception? current = null;
        for (int i = depth; i > 0; i--)
        {
            current = new Exception($"{baseMessage} {i}", current);
        }
        return current ?? new Exception("No exceptions created");
    }

    /// <summary>
    /// Asserts that a ProblemDetails has the expected structure and values.
    /// </summary>
    public static void AssertProblemDetails(
        ProblemDetails problemDetails,
        int? expectedStatus = null,
        string? expectedTitle = null,
        string? expectedDetail = null,
        string? expectedAppCode = null,
        bool shouldHaveTraceId = true)
    {
        if (expectedStatus.HasValue)
        {
            problemDetails.Status.Should().Be(expectedStatus.Value);
        }

        if (expectedTitle != null)
        {
            problemDetails.Title.Should().Be(expectedTitle);
        }

        if (expectedDetail != null)
        {
            problemDetails.Detail.Should().Be(expectedDetail);
        }

        if (expectedAppCode != null)
        {
            problemDetails.Extensions.Should().ContainKey("code");
            problemDetails.Extensions["code"].Should().Be(expectedAppCode);
        }

        if (shouldHaveTraceId)
        {
            problemDetails.Extensions.Should().ContainKey("traceId");
            problemDetails.Extensions["traceId"].Should().NotBeNull();
        }
    }

    /// <summary>
    /// Asserts that HTML content is properly encoded and contains no secrets.
    /// </summary>
    public static void AssertHtmlIsSafe(string html, params string[] secrets)
    {
        // Check that secrets are not present
        foreach (var secret in secrets)
        {
            html.Should().NotContain(secret);
        }

        // Check that HTML special characters are encoded
        html.Should().NotContainEquivalentOf("<script>");
        
        // Basic check that common sensitive patterns are not present
        html.Should().NotContainEquivalentOf("Authorization:");
        html.Should().NotContainEquivalentOf("Bearer ");
        html.Should().NotContainEquivalentOf("Cookie:");
    }

    /// <summary>
    /// Asserts that headers are properly redacted.
    /// </summary>
    public static void AssertHeadersRedacted(Dictionary<string, string> headers, params string[] sensitiveHeaderNames)
    {
        foreach (var sensitiveHeader in sensitiveHeaderNames)
        {
            if (headers.ContainsKey(sensitiveHeader))
            {
                headers[sensitiveHeader].Should().Be("[REDACTED]");
            }
        }
    }

    /// <summary>
    /// Creates an IOptions wrapper for ErrorHandlingOptions.
    /// </summary>
    public static IOptions<ErrorHandlingOptions> CreateOptions(ErrorHandlingOptions options)
    {
        var optionsWrapper = Substitute.For<IOptions<ErrorHandlingOptions>>();
        optionsWrapper.Value.Returns(options);
        return optionsWrapper;
    }

    /// <summary>
    /// Sets up Activity.Current with a trace ID (for testing trace ID extraction).
    /// </summary>
    public static Activity? SetupActivity(string? traceId = null)
    {
        var activity = new Activity("TestActivity");
        if (traceId != null)
        {
            activity.SetParentId(traceId);
        }
        activity.Start();
        return activity;
    }
}
