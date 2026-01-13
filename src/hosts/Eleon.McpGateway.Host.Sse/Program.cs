using Eleon.McpGateway.Module;
using EleonsoftSdk.modules.Helpers.Module;

namespace Eleon.McpGateway.Host.Sse;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        return await EleonsoftWebApplication.HostWebApplicationAsync<McpGatewayHostModule>(args);
    }
}
