using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Backends;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

namespace Eleon.McpGateway.Module.Infrastructure.Sessions;

public sealed class SshBackendFactory : IMcpBackendFactory
{
    private readonly SshBackendSettings settings;
    private readonly ILogger<SshMcpBackend> logger;

    public SshBackendFactory(
        SshBackendSettings settings,
        ILogger<SshMcpBackend> logger)
    {
        this.settings = settings;
        this.logger = logger;
    }

    public Task<IMcpBackend> CreateAsync(string backendName, CancellationToken cancellationToken)
    {
        var backend = new SshMcpBackend(settings, logger);
        return Task.FromResult<IMcpBackend>(backend);
    }
}

