using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace VPortal.EntityFrameworkCore;

[ExposeServices(typeof(Common.Module.Migrations.IVPortalDbSchemaMigrator))]
public class EntityFrameworkCoreVPortalDbSchemaMigrator
    : Common.Module.Migrations.IVPortalDbSchemaMigrator, ITransientDependency
{
  private readonly IServiceProvider _serviceProvider;

  public EntityFrameworkCoreVPortalDbSchemaMigrator(IServiceProvider serviceProvider)
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

    var dbContextType =
        //_serviceProvider.GetRequiredService<ICurrentTenant>().IsAvailable
        //? typeof(VPortalTenantDbContext)
        //:
        typeof(VPortalDbContext);

    await ((DbContext)_serviceProvider.GetRequiredService(dbContextType))
        .Database
        .MigrateAsync();
  }
}
