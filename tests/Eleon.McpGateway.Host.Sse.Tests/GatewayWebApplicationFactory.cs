using System.Collections.Generic;
using Eleon.McpGateway.Host.Sse;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eleon.McpGateway.Host.Sse.Tests;

internal sealed class GatewayWebApplicationFactory(params IMcpBackend[] backends)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MCP_GATEWAY_PORT"] = "0"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IMcpBackend>();
            services.RemoveAll<IMcpBackendRegistry>();

            foreach (var backend in backends)
            {
                services.AddSingleton(backend);
            }

            services.AddSingleton<IMcpBackendRegistry>(_ => new McpBackendRegistry(backends));
        });
    }
}
