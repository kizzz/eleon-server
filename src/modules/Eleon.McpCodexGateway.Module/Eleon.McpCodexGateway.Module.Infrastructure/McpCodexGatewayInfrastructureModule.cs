using Eleon.McpCodexGateway.Module.Application.Contracts.Services;
using Eleon.McpCodexGateway.Module.Application.Services;
using Eleon.McpCodexGateway.Module.Infrastructure.Services;
using Eleon.Mcp.Infrastructure.Streaming;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Eleon.McpCodexGateway.Module.Infrastructure;

[DependsOn(typeof(McpCodexGatewayApplicationModule))]
public class McpCodexGatewayInfrastructureModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<ICodexProcessOptionsProvider, CodexProcessOptionsProvider>();
        context.Services.AddSingleton<StreamPipe>();
        context.Services.AddSingleton<ErrorStreamForwarder>();
        context.Services.AddSingleton<CodexProcessProxy>();
        context.Services.AddHostedService<CodexProxyHostedService>();
    }
}

