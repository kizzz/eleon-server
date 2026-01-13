using Eleon.JsonRpc.Stdio;
using Eleon.McpSshGateway.Module;
using Eleon.McpSshGateway.Host.Stdio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
});

await builder.Services.AddApplicationAsync<McpSshGatewayHostModule>();

builder.Services.AddSingleton<JsonRpcStdioServer>();
builder.Services.AddSingleton<McpJsonRpcHandler>();

using var host = builder.Build();
await host.InitializeAsync();

var server = host.Services.GetRequiredService<JsonRpcStdioServer>();
var handler = host.Services.GetRequiredService<McpJsonRpcHandler>();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

using var cts = CancellationTokenSource.CreateLinkedTokenSource(lifetime.ApplicationStopping);
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cts.Cancel();
};

var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("McpHost");
logger.LogInformation("Eleon MCP SSH Gateway host started");

await server.RunAsync(handler, cts.Token).ConfigureAwait(false);
logger.LogInformation("Eleon MCP SSH Gateway host stopped");
