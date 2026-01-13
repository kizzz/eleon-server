using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Eleon.McpCodexGateway.Module;

[DependsOn(
    typeof(McpCodexGatewayDomainModule),
    typeof(McpCodexGatewayApplicationContractsModule),
    typeof(AbpDddApplicationModule))]
public class McpCodexGatewayApplicationModule : AbpModule
{
}

