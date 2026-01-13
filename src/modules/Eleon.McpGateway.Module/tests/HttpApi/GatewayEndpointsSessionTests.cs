using System.Text;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Application.Contracts.Services;
using Eleon.McpGateway.Module.HttpApi.Controllers;
using Eleon.McpGateway.Module.Infrastructure;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.Test.HttpApi;

public sealed class GatewayEndpointsSessionTests
{
    [Fact]
    public async Task AcceptMessageAsync_ExtractsMcpSessionIdHeader_AndPassesToDispatcher()
    {
        var dispatcher = new RecordingDispatcher();
        var coordinator = new SseConnectionCoordinator();
        var options = Options.Create(new McpGatewayOptions { BasePath = "/sse" });
        var controller = new GatewayEndpoints(
            dispatcher,
            coordinator,
            options,
            NullLogger<GatewayEndpoints>.Instance);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/sse";
        httpContext.Request.Headers["Mcp-Session-Id"] = "test-session-123";
        httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("""{"id":1,"method":"test"}"""));
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = await controller.AcceptMessageAsync("sse", null, CancellationToken.None);

        result.Should().BeOfType<AcceptedResult>();
        dispatcher.LastSessionId.Should().Be("test-session-123");
        dispatcher.ForwardCallCount.Should().Be(1);
    }

    [Fact]
    public async Task AcceptMessageAsync_WorksWithoutSessionHeader_BackwardCompatible()
    {
        var dispatcher = new RecordingDispatcher();
        var coordinator = new SseConnectionCoordinator();
        var options = Options.Create(new McpGatewayOptions { BasePath = "/sse" });
        var controller = new GatewayEndpoints(
            dispatcher,
            coordinator,
            options,
            NullLogger<GatewayEndpoints>.Instance);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/sse";
        httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("""{"id":1,"method":"test"}"""));
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = await controller.AcceptMessageAsync("sse", null, CancellationToken.None);

        result.Should().BeOfType<AcceptedResult>();
        dispatcher.LastSessionId.Should().BeNull();
        dispatcher.ForwardCallCount.Should().Be(1);
    }

    [Fact]
    public async Task StreamSseAsync_ExtractsMcpSessionIdHeader_AndPassesToDispatcher()
    {
        var dispatcher = new RecordingDispatcher();
        var coordinator = new SseConnectionCoordinator();
        var options = Options.Create(new McpGatewayOptions { BasePath = "/sse" });
        var controller = new GatewayEndpoints(
            dispatcher,
            coordinator,
            options,
            NullLogger<GatewayEndpoints>.Instance);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/sse";
        httpContext.Request.Headers["Mcp-Session-Id"] = "test-session-456";
        httpContext.RequestAborted = new CancellationTokenSource().Token;
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = await controller.StreamSseAsync("sse", null, CancellationToken.None);

        result.Should().BeOfType<EmptyResult>();
        dispatcher.LastStreamSessionId.Should().Be("test-session-456");
        dispatcher.StreamCallCount.Should().Be(1);
    }

    [Fact]
    public async Task StreamSseAsync_WorksWithoutSessionHeader_BackwardCompatible()
    {
        var dispatcher = new RecordingDispatcher();
        var coordinator = new SseConnectionCoordinator();
        var options = Options.Create(new McpGatewayOptions { BasePath = "/sse" });
        var controller = new GatewayEndpoints(
            dispatcher,
            coordinator,
            options,
            NullLogger<GatewayEndpoints>.Instance);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/sse";
        httpContext.RequestAborted = new CancellationTokenSource().Token;
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = await controller.StreamSseAsync("sse", null, CancellationToken.None);

        result.Should().BeOfType<EmptyResult>();
        dispatcher.LastStreamSessionId.Should().BeNull();
        dispatcher.StreamCallCount.Should().Be(1);
    }

    private sealed class RecordingDispatcher : IMcpGatewayDispatcher
    {
        public string? LastSessionId { get; private set; }
        public string? LastStreamSessionId { get; private set; }
        public int ForwardCallCount { get; private set; }
        public int StreamCallCount { get; private set; }

        public string ResolveBackendNameOrDefault(string? backendName) => backendName ?? "default";

        public void EnsureBackendExists(string backendName) { }

        public Task ForwardAsync(JsonNode payload, string backendName, CancellationToken cancellationToken)
        {
            return ForwardAsync(payload, backendName, null, cancellationToken);
        }

        public Task ForwardAsync(JsonNode payload, string backendName, string? sessionId, CancellationToken cancellationToken)
        {
            LastSessionId = sessionId;
            ForwardCallCount++;
            return Task.CompletedTask;
        }

        public IAsyncEnumerable<JsonNode> GetOutboundStream(string backendName, CancellationToken cancellationToken)
        {
            return GetOutboundStream(backendName, null, cancellationToken);
        }

        public async IAsyncEnumerable<JsonNode> GetOutboundStream(string backendName, string? sessionId, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            LastStreamSessionId = sessionId;
            StreamCallCount++;
            await Task.CompletedTask;
            yield break;
        }
    }
}

