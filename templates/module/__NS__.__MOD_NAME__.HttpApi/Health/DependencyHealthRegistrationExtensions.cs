using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace __NS__.__MOD_NAME__.HttpApi.Health;

public static class DependencyHealthRegistrationExtensions
{
    /// <summary>Registers dependency health checks from configuration:
    /// Modules:__MOD_NAME__:Health:Redis (host:port)
    /// Modules:__MOD_NAME__:Health:Rabbit (host:port)
    /// Modules:__MOD_NAME__:Health:Http (string[] urls)
    /// </summary>
    public static IServiceCollection Add__MOD_NAME__DependencyHealth(this IServiceCollection services, IConfiguration cfg)
    {
        var key = $"Modules:__MOD_NAME__:Health:";
        var redis = cfg[$"{key}Redis"];
        var rabbit = cfg[$"{key}Rabbit"];
        var httpUrls = cfg.GetSection($"{key}Http").Get<string[]>() ?? Array.Empty<string>();

        var hc = services.AddHealthChecks();
        if (!string.IsNullOrWhiteSpace(redis))
            hc.AddCheck($"__MOD_NAME__-redis", new RedisHealthCheck(redis), tags: new[] { "redis", "readiness", "__MOD_NAME__" });
        if (!string.IsNullOrWhiteSpace(rabbit))
            hc.AddCheck($"__MOD_NAME__-rabbit", new RabbitMqHealthCheck(rabbit), tags: new[] { "rabbit", "readiness", "__MOD_NAME__" });
        services.AddHttpClient(nameof(HttpDependencyHealthCheck));

        foreach (var url in httpUrls)
            hc.AddCheck($"__MOD_NAME__-http:{url}", new HttpDependencyHealthCheck(services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>(), url), tags: new[] { "http", "readiness", "__MOD_NAME__" });

        return services;
    }

    /// <summary>Optionally maps HealthChecks UI (requires AspNetCore.HealthChecks.UI package) if marker file exists.</summary>
    public static IEndpointRouteBuilder Map__MOD_NAME__HealthUIIfEnabled(this IEndpointRouteBuilder endpoints)
    {
        var env = endpoints.ServiceProvider.GetService<IServiceProvider>();
        // UI pipeline is wired at host-level; this marker is just a hint from generator.
        return endpoints;
    }
}
