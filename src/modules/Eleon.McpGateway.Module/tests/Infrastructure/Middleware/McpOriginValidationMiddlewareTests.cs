using System.Net;
using System.Net.Http;
using System.Text;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Eleon.McpGateway.Module.Infrastructure.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.Test.Infrastructure.Middleware;

public sealed class McpOriginValidationMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_OnlyValidatesMcpPaths()
    {
        var options = Options.Create(new McpStreamableOptions
        {
            AllowedOrigins = new[] { "https://allowed.com" }
        });
        var middleware = new McpOriginValidationMiddleware(
            context => Task.CompletedTask,
            options,
            NullLogger<McpOriginValidationMiddleware>.Instance);

        var context = CreateContext("/sse", "https://unauthorized.com");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200); // Should pass through (not /mcp)
    }

    [Fact]
    public async Task InvokeAsync_AllowsRequests_WhenNoOriginsConfigured()
    {
        var options = Options.Create(new McpStreamableOptions
        {
            AllowedOrigins = Array.Empty<string>()
        });
        var middleware = new McpOriginValidationMiddleware(
            context => Task.CompletedTask,
            options,
            NullLogger<McpOriginValidationMiddleware>.Instance);

        var context = CreateContext("/mcp", "https://any-origin.com");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200); // Should pass through
    }

    [Fact]
    public async Task InvokeAsync_RejectsUnauthorizedOrigin_With403()
    {
        var options = Options.Create(new McpStreamableOptions
        {
            AllowedOrigins = new[] { "https://allowed.com" }
        });
        var middleware = new McpOriginValidationMiddleware(
            context => Task.CompletedTask,
            options,
            NullLogger<McpOriginValidationMiddleware>.Instance);

        var context = CreateContext("/mcp", "https://unauthorized.com");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(403);
        var body = await ReadResponseBody(context);
        body.Should().Contain("Origin not allowed");
    }

    [Fact]
    public async Task InvokeAsync_LogsCorrelationId_OnRejection()
    {
        var logger = new TestLogger<McpOriginValidationMiddleware>();
        var options = Options.Create(new McpStreamableOptions
        {
            AllowedOrigins = new[] { "https://allowed.com" }
        });
        var middleware = new McpOriginValidationMiddleware(
            context => Task.CompletedTask,
            options,
            logger);

        var context = CreateContext("/mcp", "https://unauthorized.com");
        var correlationId = context.TraceIdentifier;

        await middleware.InvokeAsync(context);

        logger.Logs.Should().Contain(log =>
            log.Message.Contains("Rejected request from unauthorized origin") &&
            log.Message.Contains(correlationId));
    }

    [Fact]
    public async Task InvokeAsync_WildcardPatternMatching_WorksCorrectly()
    {
        var options = Options.Create(new McpStreamableOptions
        {
            AllowedOrigins = new[] { "https://*.example.com" }
        });
        var middleware = new McpOriginValidationMiddleware(
            context => Task.CompletedTask,
            options,
            NullLogger<McpOriginValidationMiddleware>.Instance);

        // Test matching origins
        var context1 = CreateContext("/mcp", "https://sub.example.com");
        await middleware.InvokeAsync(context1);
        context1.Response.StatusCode.Should().Be(200);

        var context2 = CreateContext("/mcp", "https://another.example.com");
        await middleware.InvokeAsync(context2);
        context2.Response.StatusCode.Should().Be(200);

        // Test non-matching origin
        var context3 = CreateContext("/mcp", "https://sub.other.com");
        await middleware.InvokeAsync(context3);
        context3.Response.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task InvokeAsync_InvalidWildcardPattern_DoesNotCrash()
    {
        var logger = new TestLogger<McpOriginValidationMiddleware>();
        var options = Options.Create(new McpStreamableOptions
        {
            AllowedOrigins = new[] { "https://[invalid" } // Invalid regex pattern
        });
        var middleware = new McpOriginValidationMiddleware(
            context => Task.CompletedTask,
            options,
            logger);

        var context = CreateContext("/mcp", "https://test.com");

        // Should not crash, should reject the request
        await middleware.InvokeAsync(context);
        context.Response.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task InvokeAsync_MissingOriginHeader_IsAllowed()
    {
        var options = Options.Create(new McpStreamableOptions
        {
            AllowedOrigins = new[] { "https://allowed.com" }
        });
        var middleware = new McpOriginValidationMiddleware(
            context => Task.CompletedTask,
            options,
            NullLogger<McpOriginValidationMiddleware>.Instance);

        var context = CreateContext("/mcp", origin: null);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200); // Should pass through
    }

    [Fact]
    public async Task InvokeAsync_ExactMatch_IsCaseInsensitive()
    {
        var options = Options.Create(new McpStreamableOptions
        {
            AllowedOrigins = new[] { "https://Allowed.COM" }
        });
        var middleware = new McpOriginValidationMiddleware(
            context => Task.CompletedTask,
            options,
            NullLogger<McpOriginValidationMiddleware>.Instance);

        var context = CreateContext("/mcp", "https://allowed.com");

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200); // Should match (case-insensitive)
    }

    private static HttpContext CreateContext(string path, string? origin)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        if (origin != null)
        {
            context.Request.Headers["Origin"] = origin;
        }
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        private readonly List<LogEntry> logs = new();

        public IReadOnlyList<LogEntry> Logs => logs.AsReadOnly();

        public IDisposable BeginScope<TState>(TState state) => NullDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            logs.Add(new LogEntry
            {
                LogLevel = logLevel,
                EventId = eventId,
                Message = formatter(state, exception),
                Exception = exception
            });
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
}
