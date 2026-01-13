using Eleon.McpSshGateway.Module.Application.Contracts.Dtos;
using Eleon.McpSshGateway.Module.Application.Contracts.Exceptions;
using Eleon.McpSshGateway.Module.Application.Services;
using Eleon.McpSshGateway.Module.Domain.Entities;
using Eleon.McpSshGateway.Module.Domain.Repositories;
using Eleon.McpSshGateway.Module.Domain.Services;
using Eleon.McpSshGateway.Module.Domain.ValueObjects;
using Eleon.Ssh;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eleon.McpSshGateway.Module.Test.Application;

public sealed class SshCommandAppServiceTests
{
    private readonly FakeHostRepository hostRepository = new();
    private readonly FakeCommandRunner commandRunner = new();
    private readonly FakeAuditRepository auditRepository = new();
    private readonly CommandPolicyService policyService = new();
    private readonly SshCommandAppService appService;

    public SshCommandAppServiceTests()
    {
        appService = new SshCommandAppService(hostRepository, commandRunner, policyService, auditRepository, NullLogger<SshCommandAppService>.Instance);
    }

    [Fact]
    public async Task Throws_When_Host_Not_Found()
    {
        Func<Task> act = () => appService.ExecuteAsync(new ExecuteCommandInput { HostId = "missing", Command = "ls" }, CancellationToken.None);

        await act.Should().ThrowAsync<HostNotFoundException>();
    }

    [Fact]
    public async Task Throws_When_Command_Denied()
    {
        hostRepository.Host = CreateHost(allow: new[] { "ls *" }, deny: new[] { "rm *" });

        Func<Task> act = () => appService.ExecuteAsync(new ExecuteCommandInput { HostId = hostRepository.Host.Id, Command = "rm file" }, CancellationToken.None);

        await act.Should().ThrowAsync<CommandRejectedException>();
    }

    [Fact]
    public async Task Executes_Command_When_Allowed()
    {
        hostRepository.Host = CreateHost(allow: new[] { "ls *" });
        var input = new ExecuteCommandInput { HostId = hostRepository.Host.Id, Command = "ls /tmp", TimeoutSeconds = 5 };

        var result = await appService.ExecuteAsync(input, CancellationToken.None);

        result.ExitCode.Should().Be(0);
        result.Stdout.Should().Be("ok");
        auditRepository.Audits.Should().HaveCount(1);
        commandRunner.ExecutedCommands.Should().ContainSingle(cmd => cmd.command == input.Command);
    }

    private static SshHost CreateHost(string[]? allow = null, string[]? deny = null)
    {
        return new SshHost("host", "Host", "localhost", 22, "user", SshCredential.PasswordCredential("secret"), allowPatterns: allow, denyPatterns: deny);
    }

    private sealed class FakeHostRepository : IHostRepository
    {
        public SshHost Host { get; set; } = null!;

        public Task<SshHost?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Host?.Id == id ? Host : null);
        }

        public Task<IReadOnlyList<SshHost>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<SshHost>>(Host is null ? Array.Empty<SshHost>() : new[] { Host });
        }
    }

    private sealed class FakeCommandRunner : ISshCommandRunner
    {
        public List<(SshConnectionInfo connection, string command)> ExecutedCommands { get; } = new();

        public Task<SshCommandResult> RunAsync(SshConnectionInfo connection, string command, TimeSpan timeout, CancellationToken cancellationToken)
        {
            ExecutedCommands.Add((connection, command));
            return Task.FromResult(new SshCommandResult(0, "ok", string.Empty, TimeSpan.FromMilliseconds(10)));
        }
    }

    private sealed class FakeAuditRepository : ICommandAuditRepository
    {
        public List<CommandAudit> Audits { get; } = new();

        public Task AddAsync(CommandAudit audit, CancellationToken cancellationToken = default)
        {
            Audits.Add(audit);
            return Task.CompletedTask;
        }
    }
}

