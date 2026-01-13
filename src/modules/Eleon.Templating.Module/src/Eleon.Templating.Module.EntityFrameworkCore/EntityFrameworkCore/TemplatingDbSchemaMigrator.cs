using Common.Module.Migrations;
using Eleon.Templating.Module.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Migrations.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Eleon.Templating.Module.EntityFrameworkCore;
[ExposeServices(typeof(IVPortalDbSchemaMigrator))]
public class TemplatingDbSchemaMigrator : IVPortalDbSchemaMigrator, ITransientDependency
{
  private readonly IServiceProvider _serviceProvider;

  public TemplatingDbSchemaMigrator(IServiceProvider serviceProvider)
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
    var dbContextType = typeof(TemplatingDbContext);

    await ((DbContext)_serviceProvider.GetRequiredService(dbContextType))
        .Database
        .MigrateAsync();
  }
}
