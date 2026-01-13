using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Eleon.McpGateway.Module.Infrastructure.Sessions;
using Eleon.McpGateway.Module.Test.HttpApi.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.Test.Infrastructure.Sessions;

public sealed class McpRequestCorrelationServiceTests
{
    [Fact]
    public void RegisterPendingRequest_AddsToSessionState()
    {
        var registry = CreateRegistry();
        var service = CreateService(registry);
        var sessionInfo = registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None).GetAwaiter().GetResult();

        var tcs = new TaskCompletionSource<JsonNode>();
        service.RegisterPendingRequest(sessionInfo.SessionId, "req-1", tcs);

        var state = ((McpSessionRegistry)registry).TryGetSessionState(sessionInfo.SessionId);
        state.Should().NotBeNull();
        state!.PendingRequests.Should().ContainKey("req-1");
    }

    [Fact]
    public async Task WaitForResponseAsync_ReturnsResponse_WhenCompleted()
    {
        var registry = CreateRegistry();
        var service = CreateService(registry);
        var sessionInfo = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);

        var tcs = new TaskCompletionSource<JsonNode>();
        service.RegisterPendingRequest(sessionInfo.SessionId, "req-1", tcs);

        var response = JsonNode.Parse("""{"jsonrpc":"2.0","id":"req-1","result":{"success":true}}""")!;
        service.CompleteRequest(sessionInfo.SessionId, "req-1", response);

        var result = await service.WaitForResponseAsync(sessionInfo.SessionId, "req-1", CancellationToken.None);
        result.Should().NotBeNull();
        result!["id"]!.ToString().Should().Be("req-1");
    }

    [Fact]
    public async Task WaitForResponseAsync_TimesOut_AfterRequestTimeout()
    {
        var options = Options.Create(new McpStreamableOptions
        {
            RequestTimeout = TimeSpan.FromMilliseconds(100)
        });
        var registry = CreateRegistry();
        var service = new McpRequestCorrelationService(registry, options, NullLogger<McpRequestCorrelationService>.Instance);
        var sessionInfo = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);

        var tcs = new TaskCompletionSource<JsonNode>();
        service.RegisterPendingRequest(sessionInfo.SessionId, "req-1", tcs);

        // Don't complete the request - should timeout
        var result = await service.WaitForResponseAsync(sessionInfo.SessionId, "req-1", CancellationToken.None);

        result.Should().NotBeNull();
        result!["error"]!.Should().NotBeNull();
        result["error"]!["message"]!.ToString().Should().Contain("timed out");
    }

    [Fact]
    public async Task WaitForResponseAsync_RespectsCancellationToken()
    {
        var registry = CreateRegistry();
        var service = CreateService(registry);
        var sessionInfo = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);

        var tcs = new TaskCompletionSource<JsonNode>();
        service.RegisterPendingRequest(sessionInfo.SessionId, "req-1", tcs);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(50);

        var act = async () => await service.WaitForResponseAsync(sessionInfo.SessionId, "req-1", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public void CompleteRequest_SetsResult_OnTaskCompletionSource()
    {
        var registry = CreateRegistry();
        var service = CreateService(registry);
        var sessionInfo = registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None).GetAwaiter().GetResult();

        var tcs = new TaskCompletionSource<JsonNode>();
        service.RegisterPendingRequest(sessionInfo.SessionId, "req-1", tcs);

        var response = JsonNode.Parse("""{"jsonrpc":"2.0","id":"req-1","result":{"value":42}}""")!;
        service.CompleteRequest(sessionInfo.SessionId, "req-1", response);

        tcs.Task.IsCompletedSuccessfully.Should().BeTrue();
        tcs.Task.Result["result"]!["value"]!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void RegisterPendingRequest_DuplicateId_ThrowsException()
    {
        var registry = CreateRegistry();
        var service = CreateService(registry);
        var sessionInfo = registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None).GetAwaiter().GetResult();

        var tcs1 = new TaskCompletionSource<JsonNode>();
        service.RegisterPendingRequest(sessionInfo.SessionId, "req-1", tcs1);

        var tcs2 = new TaskCompletionSource<JsonNode>();
        var act = () => service.RegisterPendingRequest(sessionInfo.SessionId, "req-1", tcs2);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already pending*");
    }

    [Fact]
    public void WaitForResponseAsync_RequestIdNotFound_ThrowsException()
    {
        var registry = CreateRegistry();
        var service = CreateService(registry);
        var sessionInfo = registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None).GetAwaiter().GetResult();

        var act = () => service.WaitForResponseAsync(sessionInfo.SessionId, "non-existent", CancellationToken.None).GetAwaiter().GetResult();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public void CancelRequest_RemovesPendingRequest()
    {
        var registry = CreateRegistry();
        var service = CreateService(registry);
        var sessionInfo = registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None).GetAwaiter().GetResult();

        var tcs = new TaskCompletionSource<JsonNode>();
        service.RegisterPendingRequest(sessionInfo.SessionId, "req-1", tcs);

        service.CancelRequest(sessionInfo.SessionId, "req-1");

        var state = ((McpSessionRegistry)registry).TryGetSessionState(sessionInfo.SessionId);
        state!.PendingRequests.Should().NotContainKey("req-1");
        tcs.Task.IsCanceled.Should().BeTrue();
    }

    private static McpSessionRegistry CreateRegistry()
    {
        var factory = new FakeBackendFactoryForConcurrency("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        return new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);
    }

    private static McpRequestCorrelationService CreateService(McpSessionRegistry registry)
    {
        var options = Options.Create(new McpStreamableOptions());
        return new McpRequestCorrelationService(registry, options, NullLogger<McpRequestCorrelationService>.Instance);
    }
}
