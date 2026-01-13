using Eleon.Mcp.Abstractions;
using Eleon.McpSshGateway.Module.Application.Contracts.Services;
using Eleon.McpSshGateway.Module.Application.Mcp;
using Eleon.McpSshGateway.Module.Application.Mcp.Tools;
using Eleon.McpSshGateway.Module.Application.Services;
using Eleon.McpSshGateway.Module.Domain.Repositories;
using Eleon.McpSshGateway.Module.Domain.Services;
using Eleon.McpSshGateway.Module.Infrastructure.Audit;
using Eleon.McpSshGateway.Module.Infrastructure.HostCatalog;
using Eleon.Ssh;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Eleon.McpSshGateway.Module.Infrastructure;

[DependsOn(typeof(McpSshGatewayApplicationModule))]
public class McpSshGatewayInfrastructureModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        services.AddSingleton<CommandPolicyService>();
        services.AddSingleton<IHostRepository, FileHostRepository>();
        services.AddSingleton<ICommandAuditRepository, FileCommandAuditRepository>();
        services.AddSingleton<ISshCommandRunner, SshNetCommandRunner>();
        services.AddTransient<ISshCommandAppService, SshCommandAppService>();
        services.AddTransient<IHostCatalogAppService, HostCatalogAppService>();
        services.AddSingleton<IMcpTool, SshExecuteTool>();
        services.AddSingleton<IMcpTool, SshListHostsTool>();
        services.AddSingleton<IMcpTool, SshDescribeHostTool>();
        services.AddSingleton<IMcpToolRegistry, McpToolRegistry>();
        services.AddSingleton<IMcpDispatcher, McpDispatcher>();
        services.AddSingleton<McpService>();
        services.AddOptions<FileHostRepositoryOptions>();
        services.AddOptions<FileCommandAuditRepositoryOptions>();
    }
}

