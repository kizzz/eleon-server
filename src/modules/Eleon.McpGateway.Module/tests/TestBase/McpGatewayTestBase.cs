using System;
using System.Collections.Generic;
using System.Linq;
using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Application.Contracts.Services;
using Eleon.McpGateway.Module.Application.Services;
using Eleon.McpGateway.Module.Infrastructure.Backends;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Eleon.McpGateway.Module.Infrastructure;
using Eleon.McpGateway.Module.Infrastructure.Middleware;
using Eleon.McpGateway.Module.Infrastructure.Sessions;
using Eleon.Logging.Lib.VportalLogging;
using Eleon.Mcp.Infrastructure.Paths;
using Eleon.TestsBase.Lib.TestBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Eleon.McpGateway.Module.Test.TestBase;

/// <summary>
/// Base class for MCP Gateway integration tests that provides HttpClient via TestServer.
/// Creates TestServer with its own services, but resolves business logic services from ABP.
/// </summary>
public abstract class McpGatewayTestBase : AbpIntegratedTestBase<McpGatewayTestModule>
{
    private readonly List<IMcpBackend> _backends = new();
    private TestServer? _testServer;
    private HttpClient? _httpClient;
    private IHost? _host;
    private IServiceProvider? _testServices;

    /// <summary>
    /// Gets the HttpClient for making HTTP requests to the test server.
    /// </summary>
    protected HttpClient HttpClient
    {
        get
        {
            if (_httpClient == null)
            {
                InitializeTestServer();
            }
            return _httpClient!;
        }
    }

    protected virtual IReadOnlyDictionary<string, string?> GetConfigurationOverrides()
    {
        return new Dictionary<string, string?>();
    }

    protected T GetTestService<T>() where T : notnull
    {
        if (_testServices == null)
        {
            InitializeTestServer();
        }

        return _testServices!.GetRequiredService<T>();
    }

    /// <summary>
    /// Registers fake backends to be used in tests.
    /// Must be called before the first use of HttpClient.
    /// </summary>
    protected void RegisterBackends(params IMcpBackend[] backends)
    {
        if (_testServer != null)
        {
            throw new InvalidOperationException("Cannot register backends after test server has been initialized. Call RegisterBackends before using HttpClient.");
        }

        _backends.Clear();
        _backends.AddRange(backends);
    }

    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }

    private void InitializeTestServer()
    {
        if (_testServer != null)
        {
            return;
        }

        EnsureDefaultEnvironmentVariables();

        // Create TestServer host builder with its own services
        var testHostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .UseEnvironment("Test")
            .ConfigureAppConfiguration((_, configBuilder) =>
            {
                var overrides = GetConfigurationOverrides();
                if (overrides.Count > 0)
                {
                    configBuilder.AddInMemoryCollection(overrides);
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseTestServer();
                webBuilder.ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    // Add controllers (required for MapControllers and TestServer Application)
                    services.AddControllers()
                        .AddApplicationPart(typeof(Eleon.McpGateway.Module.HttpApi.Controllers.McpStreamableController).Assembly);

                    // Add routing (required for TestServer Application)
                    services.AddRouting();

                    var streamableOptions = BuildStreamableOptions(configuration);
                    var gatewayOptions = BuildGatewayOptions(configuration);
                    var sessionOptions = BuildSessionOptions(configuration);

                    services.AddSingleton<IOptions<McpStreamableOptions>>(Options.Create(streamableOptions));
                    services.AddSingleton<IOptions<McpGatewayOptions>>(Options.Create(gatewayOptions));
                    services.AddSingleton<IOptions<McpSessionOptions>>(Options.Create(sessionOptions));

                    // Add CORS (tests use origin validation middleware for restrictions)
                    services.AddCors(options =>
                    {
                        options.AddDefaultPolicy(policy =>
                        {
                            policy.AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowAnyOrigin()
                                .WithExposedHeaders(streamableOptions.ExposedHeaders.ToArray());
                        });
                    });

                    RegisterTestServices(services);
                });
                webBuilder.Configure(appBuilder =>
                {
                    // Configure middleware pipeline
                    appBuilder.UseRouting();
                    appBuilder.UseMiddleware<McpOriginValidationMiddleware>();
                    appBuilder.UseCors();
                    appBuilder.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            });

        // Build the host - TestServer will build its Application normally
        _host = testHostBuilder.Build();
        
        // Start the host to ensure Application is initialized
        _host.StartAsync().GetAwaiter().GetResult();
        
        _testServer = _host.GetTestServer();
        _httpClient = _testServer.CreateClient();
        _testServices = _host.Services;

        WireCorrelationEvents();
    }

    /// <summary>
    /// Registers services in TestServer's service collection.
    /// </summary>
    private void RegisterTestServices(IServiceCollection services)
    {
        services.AddVportalLogging();
        if (_backends.Count == 0)
        {
            throw new InvalidOperationException("RegisterBackends must be called before initializing the test server.");
        }

        foreach (var backend in _backends)
        {
            services.AddSingleton(backend);
        }

        services.AddSingleton<IMcpBackendRegistry>(_ => new McpBackendRegistry(_backends));

        var factories = _backends.Select(backend => new FixedBackendFactory(backend)).ToList();
        services.AddSingleton(_ => new McpBackendFactoryRegistry(factories));

        services.AddSingleton<McpSessionRegistry>();
        services.AddSingleton<IMcpSessionRegistry>(sp => sp.GetRequiredService<McpSessionRegistry>());
        services.AddSingleton<McpRequestCorrelationService>();

        services.AddSingleton<IMcpGatewayDispatcher>(sp =>
        {
            var registry = sp.GetRequiredService<IMcpBackendRegistry>();
            var sessionRegistry = sp.GetService<IMcpSessionRegistry>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<McpGatewayDispatcherAppService>>();
            return new McpGatewayDispatcherAppService(registry, sessionRegistry, logger);
        });

        services.AddSingleton<SseConnectionCoordinator>();
        services.AddHostedService<McpResponseCorrelationService>();
        services.AddHostedService<McpSessionCleanupService>();
    }

    private void WireCorrelationEvents()
    {
        if (_testServices == null)
        {
            return;
        }

        var registry = _testServices.GetService<McpSessionRegistry>();
        var correlationService = _testServices.GetService<McpResponseCorrelationService>();
        if (registry != null && correlationService != null)
        {
            registry.NotifySessionCreated += correlationService.SubscribeToSession;
        }
    }

    private static void EnsureDefaultEnvironmentVariables()
    {
        SetIfMissing(
            "MCP_ALLOWED_ORIGINS",
            "https://allowed.com,https://env1.com,https://specific.com,https://*.allowed.com,https://example.com,https://any-origin.com");
        SetIfMissing("MCP_EXPOSE_HEADERS", "Mcp-Session-Id,X-Custom-Header");
    }

    private static void SetIfMissing(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    private static McpGatewayOptions BuildGatewayOptions(IConfiguration configuration)
    {
        return new McpGatewayOptions
        {
            BasePath = GatewayPath.Normalize(configuration["MCP_GATEWAY_BASE_PATH"])
        };
    }

    private static McpSessionOptions BuildSessionOptions(IConfiguration configuration)
    {
        var options = new McpSessionOptions();
        configuration.GetSection("McpSession").Bind(options);
        return options;
    }

    private static McpStreamableOptions BuildStreamableOptions(IConfiguration configuration)
    {
        var options = new McpStreamableOptions
        {
            RequestTimeout = TimeSpan.FromSeconds(1),
            SseKeepaliveInterval = TimeSpan.FromSeconds(2)
        };

        // Allowed origins: env var overrides config
        var allowedOriginsEnv = configuration["MCP_ALLOWED_ORIGINS"];
        if (!string.IsNullOrWhiteSpace(allowedOriginsEnv))
        {
            options.AllowedOrigins = allowedOriginsEnv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }
        else
        {
            var configSection = configuration.GetSection("McpStreamable:AllowedOrigins");
            if (configSection.Exists())
            {
                options.AllowedOrigins = configSection.Get<string[]>() ?? Array.Empty<string>();
            }
        }

        // Exposed headers: env var overrides config, always include Mcp-Session-Id
        var exposedHeadersEnv = configuration["MCP_EXPOSE_HEADERS"];
        if (!string.IsNullOrWhiteSpace(exposedHeadersEnv))
        {
            var headers = exposedHeadersEnv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
            if (!headers.Contains("Mcp-Session-Id", StringComparer.OrdinalIgnoreCase))
            {
                headers.Insert(0, "Mcp-Session-Id");
            }
            options.ExposedHeaders = headers;
        }
        else
        {
            var configSection = configuration.GetSection("McpStreamable:ExposedHeaders");
            if (configSection.Exists())
            {
                var headers = configSection.Get<string[]>()?.ToList() ?? new List<string> { "Mcp-Session-Id" };
                if (!headers.Contains("Mcp-Session-Id", StringComparer.OrdinalIgnoreCase))
                {
                    headers.Insert(0, "Mcp-Session-Id");
                }
                options.ExposedHeaders = headers;
            }
            else
            {
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

        return options;
    }

    private sealed class FixedBackendFactory : IMcpBackendFactory
    {
        private readonly IMcpBackend backend;

        public FixedBackendFactory(IMcpBackend backend)
        {
            this.backend = backend;
            BackendName = backend.Name;
        }

        public string BackendName { get; }

        public Task<IMcpBackend> CreateAsync(string backendName, CancellationToken cancellationToken)
        {
            return Task.FromResult(backend);
        }
    }

    public override void Dispose()
    {
        _httpClient?.Dispose();
        _testServer?.Dispose();
        _host?.Dispose();

        foreach (var backend in _backends)
        {
            backend.DisposeAsync().AsTask().Wait();
        }

        base.Dispose();
    }
}
