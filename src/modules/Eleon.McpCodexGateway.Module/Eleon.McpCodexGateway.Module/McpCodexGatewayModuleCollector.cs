using Eleon.McpCodexGateway.Module.Infrastructure;
using Volo.Abp.Modularity;

namespace Eleon.McpCodexGateway.Module;

[DependsOn(
    typeof(McpCodexGatewayApplicationContractsModule),
    typeof(McpCodexGatewayApplicationModule),
    typeof(McpCodexGatewayInfrastructureModule))]
public class McpCodexGatewayModuleCollector : AbpModule
{
}

