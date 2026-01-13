using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace __NS__.__MOD_NAME__.HttpApi.Health;

public static class HealthRegistrationExtensions
{
    /// <summary>Registers module health checks. Call from Host during service registration.</summary>
    public static IServiceCollection Add__MOD_NAME__Health(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<ModuleHealthCheck>(name: "__MOD_NAME__-self", tags: new[] { "module", "liveness", "__MOD_NAME__" });
        return services;
    }

    /// <summary>Registers an EF Core DbContext check for readiness (optional).</summary>
    public static IServiceCollection Add__MOD_NAME__DbHealth<TDbContext>(this IServiceCollection services) where TDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        services.AddHealthChecks()
            .AddCheck<DbContextHealthCheck<TDbContext>>(name: "__MOD_NAME__-db", tags: new[] { "db", "readiness", "__MOD_NAME__" });
        return services;
    }

    /// <summary>Maps module health endpoints under /health/__mod_name__/...</summary>
    public static IEndpointRouteBuilder Map__MOD_NAME__HealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health/__mod_name__/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = r => r.Name == "__MOD_NAME__-self"
        });
        endpoints.MapHealthChecks("/health/__mod_name__/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("readiness") && r.Tags.Contains("__MOD_NAME__")
        });
        endpoints.MapHealthChecks("/health/__mod_name__/details", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (ctx, rep) =>
            {
                ctx.Response.ContentType = "application/json; charset=utf-8";
                var payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = rep.Status.ToString(),
                    checks = rep.Entries
                });
                await ctx.Response.WriteAsync(payload);
            }
        });
        return endpoints;
    }
}
