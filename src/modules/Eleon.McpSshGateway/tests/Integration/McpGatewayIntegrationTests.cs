using System.Text.Json;
using System.Text.Json.Nodes;
using Eleon.JsonRpc.Stdio;
using Eleon.McpSshGateway.Module.Domain.Entities;
using Eleon.McpSshGateway.Module.Domain.Repositories;
using Eleon.McpSshGateway.Module.Domain.ValueObjects;
using Eleon.McpSshGateway.Host.Stdio;
using Eleon.McpSshGateway.Module;
using Eleon.Ssh;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;

namespace Eleon.McpSshGateway.Module.Test.Integration;

public sealed class McpGatewayIntegrationTests
{
    [Fact]
    public async Task Handles_Initialize_List_And_Call()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddProvider(NullLoggerProvider.Instance));
        services.AddSingleton<Eleon.McpSshGateway.Host.Stdio.McpJsonRpcHandler>();
        var application = await services.AddApplicationAsync<McpSshGatewayModuleCollector>();
        services.RemoveAll<IHostRepository>();
        services.AddSingleton<IHostRepository>(_ => new InMemoryHostRepository());
        var fakeRunner = new FakeCommandRunner();
        services.RemoveAll<ISshCommandRunner>();
        services.AddSingleton<ISshCommandRunner>(fakeRunner);
        services.RemoveAll<ICommandAuditRepository>();
        services.AddSingleton<ICommandAuditRepository, CollectingAuditRepository>();
        using var provider = services.BuildServiceProvider();
        await application.InitializeAsync(provider);
        var handler = provider.GetRequiredService<Eleon.McpSshGateway.Host.Stdio.McpJsonRpcHandler>();

        var requests = string.Join('\n', new[]
        {
            "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"initialize\",\"params\":{\"clientName\":\"test\",\"clientVersion\":\"1\"}}",
            "{\"jsonrpc\":\"2.0\",\"id\":2,\"method\":\"tools/list\"}",
            "{\"jsonrpc\":\"2.0\",\"id\":3,\"method\":\"tools/call\",\"params\":{\"name\":\"ssh.execute\",\"arguments\":{\"hostId\":\"local\",\"command\":\"ls\"}}}"
        });

        var input = new StringReader(requests);
        var output = new StringWriter();
        var server = new JsonRpcStdioServer(input, output, NullLogger<JsonRpcStdioServer>.Instance);

        await server.RunAsync(handler, CancellationToken.None);

        var responses = output.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        responses.Should().HaveCount(3);

        var callResponse = JsonNode.Parse(responses[2])!.AsObject();
        callResponse["result"]!["result"]?["exitCode"]?.GetValue<int>().Should().Be(0);
        fakeRunner.ExecutedCommands.Should().ContainSingle();

        await application.ShutdownAsync();
    }

    private sealed class InMemoryHostRepository : IHostRepository
    {
        private readonly SshHost host = new(
            "local",
            "Local",
            "localhost",
            22,
            "user",
            SshCredential.AgentCredential(),
            allowPatterns: new[] { "*" });

        public Task<SshHost?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(host.Id.Equals(id, StringComparison.OrdinalIgnoreCase) ? host : null);
        }

        public Task<IReadOnlyList<SshHost>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<SshHost>>(new[] { host });
        }
    }

    private sealed class FakeCommandRunner : ISshCommandRunner
    {
        public List<string> ExecutedCommands { get; } = new();

        public Task<SshCommandResult> RunAsync(SshConnectionInfo connection, string command, TimeSpan timeout, CancellationToken cancellationToken)
        {
            ExecutedCommands.Add(command);
            return Task.FromResult(new SshCommandResult(0, "ok", string.Empty, TimeSpan.FromMilliseconds(5)));
        }
    }

    private sealed class CollectingAuditRepository : ICommandAuditRepository
    {
        public List<CommandAudit> Audits { get; } = new();

        public Task AddAsync(CommandAudit audit, CancellationToken cancellationToken = default)
        {
            Audits.Add(audit);
            return Task.CompletedTask;
        }
    }
}
