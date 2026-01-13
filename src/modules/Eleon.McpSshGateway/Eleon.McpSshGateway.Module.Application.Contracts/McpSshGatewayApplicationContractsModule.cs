using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Eleon.McpSshGateway.Module;

[DependsOn(
    typeof(McpSshGatewayDomainModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
)]
public class McpSshGatewayApplicationContractsModule : AbpModule
{
}

