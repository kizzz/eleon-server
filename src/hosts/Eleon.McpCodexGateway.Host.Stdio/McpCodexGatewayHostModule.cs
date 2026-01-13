using Eleon.McpCodexGateway.Module;
using Volo.Abp.Modularity;

namespace Eleon.McpCodexGateway.Host.Stdio;

[DependsOn(typeof(McpCodexGatewayModuleCollector))]
public class McpCodexGatewayHostModule : AbpModule
{
}

