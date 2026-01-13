using EleonCore.Modules.S3.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.Cms.Feature.Module.EntityFrameworkCore
{
    [ExposeServices(typeof(Common.Module.Migrations.IVPortalDbSchemaMigrator))]
    public class EleonS3DbSchemaMigrator : Common.Module.Migrations.IVPortalDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EleonS3DbSchemaMigrator(IServiceProvider serviceProvider)
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
            var dbContextType = typeof(S3DbContext);

            try
            {
                await ((DbContext)_serviceProvider.GetRequiredService(dbContextType))
                    .Database
                    .MigrateAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
