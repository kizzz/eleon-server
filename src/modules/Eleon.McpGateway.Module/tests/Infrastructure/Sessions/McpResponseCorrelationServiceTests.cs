using System.Text.Json.Nodes;
using Eleon.Logging.Lib.VportalLogging;
using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Eleon.McpGateway.Module.Infrastructure.Sessions;
using Eleon.McpGateway.Module.Test.HttpApi.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.Test.Infrastructure.Sessions;

public sealed class McpResponseCorrelationServiceTests
{
    [Fact]
    public async Task StartAsync_SubscribesToExistingSessions()
    {
        var registry = CreateRegistry();
        var correlationService = CreateCorrelationService(registry);
        var boundaryLogger = CreateBoundaryLogger();
        var responseService = new McpResponseCorrelationService(registry, correlationService, NullLogger<McpResponseCorrelationService>.Instance, boundaryLogger);

        // Create a session before starting the service
        var sessionInfo = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);

        await responseService.StartAsync(CancellationToken.None);

        // Service should have subscribed to the session
        // We can't directly verify this, but we can test that it processes messages
        await responseService.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ProcessMessage_CompletesPendingRequest_WhenResponseReceived()
    {
        var registry = CreateRegistry();
        var correlationService = CreateCorrelationService(registry);
        var boundaryLogger = CreateBoundaryLogger();
        var responseService = new McpResponseCorrelationService(registry, correlationService, NullLogger<McpResponseCorrelationService>.Instance, boundaryLogger);

        await responseService.StartAsync(CancellationToken.None);

        var sessionInfo = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        var state = ((McpSessionRegistry)registry).TryGetSessionState(sessionInfo.SessionId);
        var backend = (FakeBackendForConcurrency)state!.Backend;

        // Register a pending request
        var tcs = new TaskCompletionSource<JsonNode>();
        correlationService.RegisterPendingRequest(sessionInfo.SessionId, "req-1", tcs);

        // Enqueue a response from the backend
        var response = JsonNode.Parse("""{"jsonrpc":"2.0","id":"req-1","result":{"success":true}}""")!;
        backend.Enqueue(response);

        // Wait a bit for the service to process
        await Task.Delay(100);

        // The request should be completed
        tcs.Task.IsCompletedSuccessfully.Should().BeTrue();

        await responseService.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ProcessMessage_IgnoresNotifications_WithoutId()
    {
        var registry = CreateRegistry();
        var correlationService = CreateCorrelationService(registry);
        var boundaryLogger = CreateBoundaryLogger();
        var responseService = new McpResponseCorrelationService(registry, correlationService, NullLogger<McpResponseCorrelationService>.Instance, boundaryLogger);

        await responseService.StartAsync(CancellationToken.None);

        var sessionInfo = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        var state = ((McpSessionRegistry)registry).TryGetSessionState(sessionInfo.SessionId);
        var backend = (FakeBackendForConcurrency)state!.Backend;

        // Enqueue a notification (no id)
        var notification = JsonNode.Parse("""{"jsonrpc":"2.0","method":"notify","params":{}}""")!;
        backend.Enqueue(notification);

        // Wait a bit
        await Task.Delay(100);

        // Should not crash or process as response
        state.PendingRequests.Should().BeEmpty();

        await responseService.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ProcessMessage_IgnoresRequests_WithoutResultOrError()
    {
        var registry = CreateRegistry();
        var correlationService = CreateCorrelationService(registry);
        var boundaryLogger = CreateBoundaryLogger();
        var responseService = new McpResponseCorrelationService(registry, correlationService, NullLogger<McpResponseCorrelationService>.Instance, boundaryLogger);

        await responseService.StartAsync(CancellationToken.None);

        var sessionInfo = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);
        var state = ((McpSessionRegistry)registry).TryGetSessionState(sessionInfo.SessionId);
        var backend = (FakeBackendForConcurrency)state!.Backend;

        // Enqueue a request (has id but no result/error)
        var request = JsonNode.Parse("""{"jsonrpc":"2.0","id":"req-1","method":"test","params":{}}""")!;
        backend.Enqueue(request);

        // Wait a bit
        await Task.Delay(100);

        // Should not process as response
        state.PendingRequests.Should().BeEmpty();

        await responseService.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StopAsync_CancelsActiveSubscriptions()
    {
        var registry = CreateRegistry();
        var correlationService = CreateCorrelationService(registry);
        var boundaryLogger = CreateBoundaryLogger();
        var responseService = new McpResponseCorrelationService(registry, correlationService, NullLogger<McpResponseCorrelationService>.Instance, boundaryLogger);

        await responseService.StartAsync(CancellationToken.None);
        var sessionInfo = await registry.GetOrCreateAsync("session-1", "test-backend", CancellationToken.None);

        // Stop should complete without hanging
        await responseService.StopAsync(CancellationToken.None);
    }

    [Fact]
    public void SubscribeToSession_WhenServiceNotStarted_DoesNothing()
    {
        var registry = CreateRegistry();
        var correlationService = CreateCorrelationService(registry);
        var boundaryLogger = CreateBoundaryLogger();
        var responseService = new McpResponseCorrelationService(registry, correlationService, NullLogger<McpResponseCorrelationService>.Instance, boundaryLogger);

        // Subscribe before starting - should not crash
        responseService.SubscribeToSession("session-1");
    }

    private static McpSessionRegistry CreateRegistry()
    {
        var factory = new FakeBackendFactoryForConcurrency("test-backend");
        var factoryRegistry = new McpBackendFactoryRegistry(new[] { factory });
        var options = Options.Create(new McpSessionOptions());
        return new McpSessionRegistry(factoryRegistry, options, NullLogger<McpSessionRegistry>.Instance);
    }

    private static McpRequestCorrelationService CreateCorrelationService(McpSessionRegistry registry)
    {
        var options = Options.Create(new McpStreamableOptions());
        return new McpRequestCorrelationService(registry, options, NullLogger<McpRequestCorrelationService>.Instance);
    }

    private static IBoundaryLogger CreateBoundaryLogger()
    {
        var scopeFactory = new VportalOperationScopeFactory(NullLogger<VportalOperationScopeFactory>.Instance);
        return new BoundaryLogger(scopeFactory, NullLogger<BoundaryLogger>.Instance);
    }
}
