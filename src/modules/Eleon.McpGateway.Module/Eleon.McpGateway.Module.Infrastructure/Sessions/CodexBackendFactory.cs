using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Backends;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

namespace Eleon.McpGateway.Module.Infrastructure.Sessions;

public sealed class CodexBackendFactory : IMcpBackendFactory
{
    private readonly CodexBackendSettings settings;
    private readonly ILogger<CodexMcpBackend> logger;

    public CodexBackendFactory(
        CodexBackendSettings settings,
        ILogger<CodexMcpBackend> logger)
    {
        this.settings = settings;
        this.logger = logger;
    }

    public Task<IMcpBackend> CreateAsync(string backendName, CancellationToken cancellationToken)
    {
        var backend = new CodexMcpBackend(settings, logger);
        return Task.FromResult<IMcpBackend>(backend);
    }
}

