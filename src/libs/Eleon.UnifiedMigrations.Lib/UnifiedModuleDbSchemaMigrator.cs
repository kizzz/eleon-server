using Common.Module.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Migrations.Module;
using ModuleCollector.UnifiedMigrations;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.Unified.Module.EntityFrameworkCore
{
    [ExposeServices(typeof(IVPortalDbSchemaMigrator))]
    public class UnifiedModuleDbSchemaMigrator : IVPortalDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public UnifiedModuleDbSchemaMigrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the DbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope (connection string is dynamically resolved).
             */

            //var dbContextType = _serviceProvider.GetRequiredService<ICurrentTenant>().IsAvailable
            //    ? typeof(BinsTenantDbContext)
            //    : typeof(BinsDbContext);

            // var dbContext = _serviceProvider.GetService<UnifiedDbContext>();

            var dbContext = UnifiedModuleDbContextFactory.CreateLite();

            await dbContext
                .Database
                .MigrateAsync();
        }
    }
}
