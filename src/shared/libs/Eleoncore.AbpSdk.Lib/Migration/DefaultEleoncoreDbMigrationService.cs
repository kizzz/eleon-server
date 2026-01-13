using Common.Module.Migrations;
using Microsoft.Extensions.DependencyInjection;
using TenantSettings.Module.Cache;
using Volo.Abp.Data;

namespace EleonsoftSdk.modules.Migration.Module;

public class DefaultEleoncoreDbMigrationService : DefaultDbMigrationService
{
  private readonly IServiceProvider serviceProvider;

  public DefaultEleoncoreDbMigrationService(
      IDataSeeder dataSeeder,
      IEnumerable<IVPortalDbSchemaMigrator> dbSchemaMigrators,
      IServiceProvider serviceProvider) : base(serviceProvider, dataSeeder, dbSchemaMigrators)
  {
    this.serviceProvider = serviceProvider;
  }

  public override Task<List<TenantInfo>> GetTenantsAsync()
  {
    var tenants = serviceProvider.GetRequiredService<EleoncoreSdkTenantCacheService>().GetTenants();

    var result = tenants.Select(t => new TenantInfo(t.Id ?? Guid.Empty, t.Name, t.ConnectionStrings?.Select(x => x.Value)?.ToList() ?? [])).ToList();

    return Task.FromResult(result);
  }
}
