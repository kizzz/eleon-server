using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace EleoncoreMultiTenancy.Module.EntityFrameworkCore;

[DependsOn(
    typeof(EleonAbpEfDomainModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),        // or PostgreSQL
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule)
)]
public class EleonAbpEfEntityFrameworkCoreModule : AbpModule
{

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<EleonAbpDbContext>(options =>
    {
      options.ReplaceDbContext<IIdentityDbContext>();
      options.ReplaceDbContext<IPermissionManagementDbContext>();
      options.ReplaceDbContext<ISettingManagementDbContext>();
      options.ReplaceDbContext<ITenantManagementDbContext>();
      options.ReplaceDbContext<IBackgroundJobsDbContext>();
      options.ReplaceDbContext<IAuditLoggingDbContext>();
      options.ReplaceDbContext<IFeatureManagementDbContext>();

      options.AddDefaultRepositories(includeAllEntities: true);
    });
  }
}
