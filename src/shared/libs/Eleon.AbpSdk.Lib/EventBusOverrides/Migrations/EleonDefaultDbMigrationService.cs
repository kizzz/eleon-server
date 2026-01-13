using Common.Module.Migrations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.Data;

namespace EleonsoftSdk.modules.Migration.Module;

public class EleonDefaultDbMigrationService : DefaultDbMigrationService
{
  private readonly IServiceProvider _serviceProvider;

  public EleonDefaultDbMigrationService(
        IDataSeeder dataSeeder,
        IEnumerable<IVPortalDbSchemaMigrator> dbSchemaMigrators,
        IServiceProvider serviceProvider) : base(serviceProvider, dataSeeder, dbSchemaMigrators)
  {
    _serviceProvider = serviceProvider;
  }

  public override async Task<List<TenantInfo>> GetTenantsAsync()
  {
    var tenants = await _serviceProvider.GetRequiredService<TenantCacheService>().GetTenantsAsync();
    return tenants.Select(x => new TenantInfo(x.Id, x.Name, x.ConnectionStrings?.Select(cs => cs.Value).ToList() ?? new List<string>())).ToList() ?? [];
  }
}
