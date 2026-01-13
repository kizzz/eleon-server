using Common.Module.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Migrations.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.FileManager.Module.EntityFrameworkCore
{
  [ExposeServices(typeof(Common.Module.Migrations.IVPortalDbSchemaMigrator))]
  public class FileManagerDbSchemaMigrator : IVPortalDbSchemaMigrator, ITransientDependency
  {
    private readonly IServiceProvider _serviceProvider;

    public FileManagerDbSchemaMigrator(IServiceProvider serviceProvider)
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
      var dbContextType = typeof(FileManagerDbContext);

      await ((DbContext)_serviceProvider.GetRequiredService(dbContextType))
          .Database
          .MigrateAsync();
    }
  }
}
