using Eleon.McpGateway.Module.Application.Contracts.Services;
using Eleon.McpGateway.Module.Application.Services;
using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Backends;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Eleon.McpGateway.Module.Infrastructure.PathResolvers;
using Eleon.McpGateway.Module.Infrastructure.Sessions;
using Eleon.Mcp.Infrastructure.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Eleon.McpGateway.Module.Infrastructure;

[DependsOn(typeof(McpGatewayApplicationModule))]
public class McpGatewayInfrastructureModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var configuration = context.Services.GetConfiguration();

        // Configuration
        services.AddOptions<McpGatewayOptions>()
            .Configure(options =>
            {
                options.BasePath = Eleon.Mcp.Infrastructure.Paths.GatewayPath.Normalize(configuration["MCP_GATEWAY_BASE_PATH"]);
            });

        // Session configuration
        services.AddOptions<McpSessionOptions>()
            .Bind(configuration.GetSection("McpSession"))
            .ValidateDataAnnotations();

        // Streamable configuration
        services.AddOptions<McpStreamableOptions>()
            .Configure(options =>
            {
                // Read from environment variables first, then fallback to configuration
                var allowedOriginsEnv = configuration["MCP_ALLOWED_ORIGINS"];
                if (!string.IsNullOrWhiteSpace(allowedOriginsEnv))
                {
                    options.AllowedOrigins = allowedOriginsEnv
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .ToList();
                }
                else
                {
                    // Fallback to configuration section
                    var configSection = configuration.GetSection("McpStreamable:AllowedOrigins");
                    if (configSection.Exists())
                    {
                        options.AllowedOrigins = configSection.Get<string[]>() ?? Array.Empty<string>();
                    }
                }

                var exposedHeadersEnv = configuration["MCP_EXPOSE_HEADERS"];
                if (!string.IsNullOrWhiteSpace(exposedHeadersEnv))
                {
                    var headers = exposedHeadersEnv
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .ToList();
                    // Ensure Mcp-Session-Id is always included
                    if (!headers.Contains("Mcp-Session-Id", StringComparer.OrdinalIgnoreCase))
                    {
                        headers.Insert(0, "Mcp-Session-Id");
                    }
                    options.ExposedHeaders = headers;
                }
                else
                {
                    // Fallback to configuration section
                    var configSection = configuration.GetSection("McpStreamable:ExposedHeaders");
                    if (configSection.Exists())
                    {
                        var headers = configSection.Get<string[]>()?.ToList() ?? new List<string> { "Mcp-Session-Id" };
                        // Ensure Mcp-Session-Id is always included
                        if (!headers.Contains("Mcp-Session-Id", StringComparer.OrdinalIgnoreCase))
                        {
                            headers.Insert(0, "Mcp-Session-Id");
                        }
                        options.ExposedHeaders = headers;
                    }
                    else
                    {
                        // Default
                        options.ExposedHeaders = new[] { "Mcp-Session-Id" };
                    }
                }

                // Bind other properties from configuration
                var timeout = configuration["McpStreamable:RequestTimeout"];
                if (!string.IsNullOrWhiteSpace(timeout) && TimeSpan.TryParse(timeout, out var timeoutValue))
                {
                    options.RequestTimeout = timeoutValue;
                }

                var tolerantMode = configuration["McpStreamable:TolerantMode"];
                if (!string.IsNullOrWhiteSpace(tolerantMode) && bool.TryParse(tolerantMode, out var tolerantModeValue))
                {
                    options.TolerantMode = tolerantModeValue;
                }

                var keepalive = configuration["McpStreamable:SseKeepaliveInterval"];
                if (!string.IsNullOrWhiteSpace(keepalive) && TimeSpan.TryParse(keepalive, out var keepaliveValue))
                {
                    options.SseKeepaliveInterval = keepaliveValue;
                }
            })
            .ValidateDataAnnotations();

        // Backend settings
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return CodexBackendSettings.Create(config);
        });
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return SshBackendSettings.Create(config);
        });

        // Backend implementations
        services.AddSingleton<IMcpBackend, CodexMcpBackend>();
        services.AddSingleton<IMcpBackend, SshMcpBackend>();

        // Registry
        services.AddSingleton<IMcpBackendRegistry, McpBackendRegistry>();

        // Session backend factories
        services.AddSingleton<IMcpBackendFactory, CodexBackendFactory>();
        services.AddSingleton<IMcpBackendFactory, SshBackendFactory>();
        services.AddSingleton<McpBackendFactoryRegistry>();

        // Session registry
        services.AddSingleton<McpSessionRegistry>();
        services.AddSingleton<IMcpSessionRegistry>(sp => sp.GetRequiredService<McpSessionRegistry>());

        // Request correlation service
        services.AddSingleton<McpRequestCorrelationService>();

        // Response correlation service
        services.AddHostedService<McpResponseCorrelationService>();

        // Dispatcher (with session registry support)
        services.AddSingleton<IMcpGatewayDispatcher>(sp =>
        {
            var registry = sp.GetRequiredService<IMcpBackendRegistry>();
            var sessionRegistry = sp.GetService<IMcpSessionRegistry>();
            var logger = sp.GetRequiredService<ILogger<McpGatewayDispatcherAppService>>();
            return new McpGatewayDispatcherAppService(registry, sessionRegistry, logger);
        });

        // Hosted services
        services.AddHostedService<McpBackendHostedService>();
        services.AddHostedService<McpSessionCleanupService>();

        // Connection coordinator
        services.AddSingleton<SseConnectionCoordinator>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        // Wire up session creation notification after services are built
        var registry = context.ServiceProvider.GetRequiredService<McpSessionRegistry>();
        var correlationService = context.ServiceProvider.GetService<McpResponseCorrelationService>();
        if (correlationService is not null)
        {
            registry.NotifySessionCreated += correlationService.SubscribeToSession;
        }
    }
}

