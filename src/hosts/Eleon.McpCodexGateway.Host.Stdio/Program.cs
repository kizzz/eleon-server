using Eleon.McpCodexGateway.Module;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

var builder = Host.CreateApplicationBuilder(args);

await builder.Services.AddApplicationAsync<McpCodexGatewayHostModule>();

var host = builder.Build();
await host.InitializeAsync();
await host.RunAsync();
