using Eleon.McpSshGateway.Application.Dtos;
using Eleon.McpSshGateway.Application.Exceptions;
using Eleon.McpSshGateway.Domain.Entities;
using Eleon.McpSshGateway.Domain.Repositories;
using Eleon.McpSshGateway.Domain.Services;
using Eleon.McpSshGateway.Domain.ValueObjects;
using Eleon.Ssh;
using Microsoft.Extensions.Logging;

namespace Eleon.McpSshGateway.Application.Services;

public sealed class SshCommandAppService
{
    private readonly IHostRepository hostRepository;
    private readonly ISshCommandRunner commandRunner;
    private readonly CommandPolicyService policyService;
    private readonly ICommandAuditRepository auditRepository;
    private readonly ILogger<SshCommandAppService> logger;

    public SshCommandAppService(
        IHostRepository hostRepository,
        ISshCommandRunner commandRunner,
        CommandPolicyService policyService,
        ICommandAuditRepository auditRepository,
        ILogger<SshCommandAppService> logger)
    {
        this.hostRepository = hostRepository;
        this.commandRunner = commandRunner;
        this.policyService = policyService;
        this.auditRepository = auditRepository;
        this.logger = logger;
    }

    public async Task<ExecuteCommandResult> ExecuteAsync(ExecuteCommandInput input, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (string.IsNullOrWhiteSpace(input.HostId))
        {
            throw new ArgumentException("HostId is required", nameof(input));
        }

        if (string.IsNullOrWhiteSpace(input.Command))
        {
            throw new ArgumentException("Command is required", nameof(input));
        }

        var host = await hostRepository.FindByIdAsync(input.HostId, cancellationToken).ConfigureAwait(false)
                   ?? throw new HostNotFoundException(input.HostId);

        if (!host.IsEnabled)
        {
            throw new HostDisabledException(host.Id);
        }

        if (!policyService.IsAllowed(host, input.Command))
        {
            throw new CommandRejectedException($"Command '{input.Command}' is blocked by the security policy for host '{host.Id}'.");
        }

        var timeout = DetermineTimeout(input.TimeoutSeconds);
        var connectionInfo = BuildConnectionInfo(host);

        logger.LogInformation("Executing SSH command on host {HostId}", host.Id);

        var result = await commandRunner.RunAsync(connectionInfo, input.Command, timeout, cancellationToken).ConfigureAwait(false);

        var audit = CommandAudit.FromResult(host.Id, input.Command, result.ExitCode, result.Stdout, result.Stderr, result.Duration);
        await auditRepository.AddAsync(audit, cancellationToken).ConfigureAwait(false);

        return new ExecuteCommandResult(result.ExitCode, result.Stdout, result.Stderr, result.Duration);
    }

    private static TimeSpan DetermineTimeout(int? timeoutSeconds)
    {
        if (timeoutSeconds is null or <= 0)
        {
            return TimeSpan.FromSeconds(30);
        }

        var bound = Math.Clamp(timeoutSeconds.Value, 1, 300);
        return TimeSpan.FromSeconds(bound);
    }

    private static SshConnectionInfo BuildConnectionInfo(SshHost host)
    {
        var credential = host.Credential;
        return credential.Type switch
        {
            SshCredentialType.Password when string.IsNullOrEmpty(credential.Password) =>
                throw new CommandRejectedException($"Host '{host.Id}' is missing a password value."),
            SshCredentialType.PrivateKey when string.IsNullOrEmpty(credential.PrivateKeyPath) =>
                throw new CommandRejectedException($"Host '{host.Id}' is missing a private key path."),
            _ => new SshConnectionInfo
            {
                Host = host.HostName,
                Port = host.Port,
                Username = host.Username,
                AuthenticationMode = credential.Type switch
                {
                    SshCredentialType.Password => SshAuthenticationMode.Password,
                    SshCredentialType.PrivateKey => SshAuthenticationMode.PrivateKey,
                    SshCredentialType.Agent => SshAuthenticationMode.Agent,
                    _ => SshAuthenticationMode.Password
                },
                Password = credential.Password,
                PrivateKeyPath = credential.PrivateKeyPath,
                PrivateKeyPassphrase = credential.PrivateKeyPassphrase,
                KeepAliveInterval = TimeSpan.FromSeconds(30)
            }
        };
    }
}
