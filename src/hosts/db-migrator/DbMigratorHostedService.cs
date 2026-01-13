using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Migrations.Module;

namespace VPortal.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;
    private readonly IBoundaryLogger _boundaryLogger;

    public DbMigratorHostedService(
        IHostApplicationLifetime hostApplicationLifetime,
        IConfiguration configuration,
        IBoundaryLogger boundaryLogger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _configuration = configuration;
        _boundaryLogger = boundaryLogger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var _ = _boundaryLogger.Begin("HostedService DbMigratorHostedService");
        using (var application = await AbpApplicationFactory.CreateAsync<VPortalDbMigratorModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.UseAutofac();
            options.Services.AddLogging(c => c.AddSerilog());
        }))
        {
            await application.InitializeAsync();

            await application
                .ServiceProvider
                .GetRequiredService<IDbMigrationService>()
                .MigrateAsync();

            await application.ShutdownAsync();

            _hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
