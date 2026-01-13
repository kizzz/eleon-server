using Volo.Abp.Modularity;
using Eleon.McpGateway.Module.HttpApi;
using Eleon.McpGateway.Module.Infrastructure;

namespace Eleon.McpGateway.Module;

[DependsOn(
    typeof(McpGatewayApplicationContractsModule),
    typeof(McpGatewayApplicationModule),
    typeof(McpGatewayInfrastructureModule),
    typeof(McpGatewayHttpApiModule))]
public class McpGatewayModuleCollector : AbpModule
{
}

