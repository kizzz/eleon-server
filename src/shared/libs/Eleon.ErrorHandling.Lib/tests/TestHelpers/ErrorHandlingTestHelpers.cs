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

namespace Eleon.ErrorHandling.Lib.Tests.TestHelpers;

/// <summary>
/// Helpers for testing error handling scenarios.
/// </summary>
public static class ErrorHandlingTestHelpers
{
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

    public static HttpContext CreateHtmlRequestContext(string? path = "/test")
    {
        return CreateHttpContext(acceptHeader: MediaTypeNames.Text.Html, path: path);
    }

    public static HttpContext CreateJsonRequestContext(string? path = "/test")
    {
        return CreateHttpContext(acceptHeader: "application/json", path: path);
    }

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

    public static IProblemDetailsService CreateMockProblemDetailsService(bool returnsTrue = true)
    {
        var service = Substitute.For<IProblemDetailsService>();
        service.TryWriteAsync(Arg.Any<ProblemDetailsContext>())
            .Returns(new ValueTask<bool>(returnsTrue));
        return service;
    }

    public static IHostEnvironment CreateMockHostEnvironment(bool isDevelopment = false, string environmentName = "Production")
    {
        var env = Substitute.For<IHostEnvironment>();
        env.IsDevelopment().Returns(isDevelopment);
        env.EnvironmentName.Returns(environmentName);
        return env;
    }

    public static ErrorHandlingOptions CreateErrorHandlingOptions(Action<ErrorHandlingOptions>? configure = null)
    {
        var options = new ErrorHandlingOptions();
        configure?.Invoke(options);
        return options;
    }

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

    public static Exception CreateExceptionWithStatusCode(int statusCode, string message = "Test exception")
    {
        return CreateExceptionWithData(message, new Dictionary<string, object?> { { "StatusCode", statusCode } });
    }

    public static Exception CreateExceptionWithFriendlyMessage(string friendlyMessage, string message = "Test exception")
    {
        return CreateExceptionWithData(message, new Dictionary<string, object?> { { "FriendlyMessage", friendlyMessage } });
    }

    public static Exception CreateDeepExceptionChain(int depth, string baseMessage = "Level")
    {
        Exception? current = null;
        for (int i = depth; i > 0; i--)
        {
            current = new Exception($"{baseMessage} {i}", current);
        }
        return current ?? new Exception("No exceptions created");
    }

    public static IOptions<ErrorHandlingOptions> CreateOptions(ErrorHandlingOptions options)
    {
        var optionsWrapper = Substitute.For<IOptions<ErrorHandlingOptions>>();
        optionsWrapper.Value.Returns(options);
        return optionsWrapper;
    }

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
