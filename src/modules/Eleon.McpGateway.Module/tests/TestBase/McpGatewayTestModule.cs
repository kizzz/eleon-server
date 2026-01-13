using Eleon.McpGateway.Module;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace Eleon.McpGateway.Module.Test.TestBase;

/// <summary>
/// ABP test module for MCP Gateway integration tests.
/// Configures test-friendly options and allows backend injection via DI.
/// </summary>
[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(McpGatewayModuleCollector))]
public class McpGatewayTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var configuration = context.Services.GetConfiguration();

        // Configure test-friendly options
        services.Configure<McpStreamableOptions>(options =>
        {
            // Short timeouts for faster tests
            options.RequestTimeout = TimeSpan.FromSeconds(5);
            options.SseKeepaliveInterval = TimeSpan.FromSeconds(2);
            // TolerantMode can be overridden per-test via configuration
            options.TolerantMode = configuration.GetValue<bool>("McpStreamable:TolerantMode", false);
            // Allow all origins in test mode by default (can be overridden)
            options.AllowedOrigins = configuration.GetSection("McpStreamable:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            options.ExposedHeaders = configuration.GetSection("McpStreamable:ExposedHeaders").Get<string[]>() ?? new[] { "Mcp-Session-Id" };
        });

        // Configure CORS for test (allow all origins in test mode by default)
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin() // Test mode: allow all origins
                    .WithExposedHeaders("Mcp-Session-Id");
            });
        });

        // Set MCP_GATEWAY_PORT to 0 (dynamic port) for tests
        services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
        {
            options.ListenAnyIP(0);
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        // ApplicationBuilder may not be available in test contexts
        IApplicationBuilder? app = null;
        try
        {
            app = context.GetApplicationBuilder();
        }
        catch (ArgumentNullException)
        {
            // No application builder in test context.
            return;
        }
        
        app.UseCors();
    }
}
