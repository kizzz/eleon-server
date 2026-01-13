using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Eleon.McpCodexGateway.Module;

[DependsOn(typeof(AbpDddDomainModule))]
public class McpCodexGatewayDomainModule : AbpModule
{
}

