using Eleon.McpSshGateway.Module.Infrastructure;
using Volo.Abp.Modularity;

namespace Eleon.McpSshGateway.Module;

[DependsOn(
    typeof(McpSshGatewayApplicationContractsModule),
    typeof(McpSshGatewayApplicationModule),
    typeof(McpSshGatewayInfrastructureModule))]
public class McpSshGatewayModuleCollector : AbpModule
{
}

