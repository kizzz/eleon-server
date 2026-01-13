using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace __NS__.__MOD_NAME__.HttpApi.Health;

/// <summary>Generic EF Core DbContext health check (optional; only used if the module registers a DbContext).</summary>
public sealed class DbContextHealthCheck<TDbContext> : IHealthCheck where TDbContext : DbContext
{
    private readonly IServiceProvider _sp;
    public DbContextHealthCheck(IServiceProvider sp) => _sp = sp;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
            // Executes a trivial query to validate connectivity & metadata
            await db.Database.CanConnectAsync(cancellationToken);
            return HealthCheckResult.Healthy($"{typeof(TDbContext).Name} connected");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"{typeof(TDbContext).Name} connectivity failed", ex);
        }
    }
}
