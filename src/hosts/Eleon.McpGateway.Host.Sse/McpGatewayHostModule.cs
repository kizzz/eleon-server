using Eleon.McpGateway.Module;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Eleon.McpGateway.Host.Sse;

[DependsOn(typeof(McpGatewayModuleCollector))]
public class McpGatewayHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        var port = configuration.GetValue("MCP_GATEWAY_PORT", 8001);
        services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
        {
            options.ListenAnyIP(port);
        });

        // Read from environment variables or configuration
        var allowedOriginsEnv = configuration["MCP_ALLOWED_ORIGINS"];
        var allowedOrigins = !string.IsNullOrWhiteSpace(allowedOriginsEnv)
            ? allowedOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : configuration.GetSection("McpStreamable:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        var exposedHeadersEnv = configuration["MCP_EXPOSE_HEADERS"];
        var exposedHeaders = !string.IsNullOrWhiteSpace(exposedHeadersEnv)
            ? exposedHeadersEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : configuration.GetSection("McpStreamable:ExposedHeaders").Get<string[]>() ?? new[] { "Mcp-Session-Id" };

        // Ensure Mcp-Session-Id is always exposed
        var exposedHeadersList = exposedHeaders.ToList();
        if (!exposedHeadersList.Contains("Mcp-Session-Id", StringComparer.OrdinalIgnoreCase))
        {
            exposedHeadersList.Insert(0, "Mcp-Session-Id");
        }

        var isDevelopment = hostingEnvironment.IsDevelopment();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders(exposedHeadersList.ToArray());

                // Environment-aware CORS policy
                if (isDevelopment)
                {
                    // Development: permissive (allow any origin)
                    policy.AllowAnyOrigin();
                }
                else
                {
                    // Production: restrictive (only allowed origins)
                    if (allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins)
                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                    }
                    else
                    {
                        // Production with no origins configured: deny all (secure default)
                        policy.SetIsOriginAllowed(_ => false);
                    }
                }
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        app.UseCors();
    }
}

