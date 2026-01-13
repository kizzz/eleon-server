using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Eleon.McpGateway.Module;

[DependsOn(
    typeof(McpGatewayDomainModule),
    typeof(McpGatewayApplicationContractsModule),
    typeof(AbpDddApplicationModule))]
public class McpGatewayApplicationModule : AbpModule
{
}

