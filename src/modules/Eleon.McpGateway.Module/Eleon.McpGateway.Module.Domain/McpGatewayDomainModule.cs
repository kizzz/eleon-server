using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Eleon.McpGateway.Module;

[DependsOn(typeof(AbpDddDomainModule))]
public class McpGatewayDomainModule : AbpModule
{
}

