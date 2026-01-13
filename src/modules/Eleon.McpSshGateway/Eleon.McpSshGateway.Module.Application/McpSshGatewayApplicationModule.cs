using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Eleon.McpSshGateway.Module;

[DependsOn(
    typeof(McpSshGatewayDomainModule),
    typeof(McpSshGatewayApplicationContractsModule),
    typeof(AbpDddApplicationModule))]
public class McpSshGatewayApplicationModule : AbpModule
{
}

