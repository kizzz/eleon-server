using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Eleon.McpSshGateway.Module;

[DependsOn(typeof(AbpDddDomainModule))]
public class McpSshGatewayDomainModule : AbpModule
{
}

