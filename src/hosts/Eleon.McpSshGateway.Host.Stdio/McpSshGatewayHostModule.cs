using Eleon.McpSshGateway.Module;
using Volo.Abp.Modularity;

namespace Eleon.McpSshGateway.Host.Stdio;

[DependsOn(typeof(McpSshGatewayModuleCollector))]
public class McpSshGatewayHostModule : AbpModule
{
}

