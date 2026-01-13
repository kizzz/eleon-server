using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Eleon.McpCodexGateway.Module;

[DependsOn(
    typeof(McpCodexGatewayDomainModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
)]
public class McpCodexGatewayApplicationContractsModule : AbpModule
{
}

