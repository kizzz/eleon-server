using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Eleon.McpGateway.Module;

[DependsOn(
    typeof(McpGatewayDomainModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
)]
public class McpGatewayApplicationContractsModule : AbpModule
{
}

