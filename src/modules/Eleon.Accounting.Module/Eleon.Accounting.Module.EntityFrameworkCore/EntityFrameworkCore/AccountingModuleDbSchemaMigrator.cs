using Common.Module.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Migrations.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.Accounting.Module.EntityFrameworkCore
{
  [ExposeServices(typeof(IVPortalDbSchemaMigrator))]
  public class AccountingModuleDbSchemaMigrator : IVPortalDbSchemaMigrator, ITransientDependency
  {
    private readonly IServiceProvider _serviceProvider;

    public AccountingModuleDbSchemaMigrator(IServiceProvider serviceProvider)
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
      var dbContextType = typeof(AccountingDbContext);

      var dbContext = ((DbContext)_serviceProvider.GetRequiredService(dbContextType));
      await dbContext.Database.MigrateAsync();

      //if (dbContext.Database.HasPendingModelChanges())
      //{
      //  _serviceProvider.GetService<ILogger<AccountingModuleDbSchemaMigrator>>()?.LogError("AccountingDbContext: Pending model changes detected.");
      //  throw new InvalidOperationException("AccountingDbContext: Has pending model changes after migration.");
      //}
    }
  }
}
