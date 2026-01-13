using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using VPortal.EventManagementModule.Module.EntityFrameworkCore;

namespace Volo.Abp.TenantManagement.EntityFrameworkCore;

[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
[ConnectionStringName(MigrationConsts.DefaultConnectionStringName)]
public class EleoncoreTenantManagementDbContext : AbpDbContext<EleoncoreTenantManagementDbContext>, IEleoncoreTenantManagementModuleDbContext
{
  public EleoncoreTenantManagementDbContext(DbContextOptions<EleoncoreTenantManagementDbContext> options)
      : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
