using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
// using Volo.Abp.IdentityServer.EntityFrameworkCore;
//using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
//using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace VPortal.EntityFrameworkCore;

[DependsOn(
    typeof(VPortalDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    // typeof(AbpIdentityServerEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule)
)]
public class VPortalEntityFrameworkCoreModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    VPortalEfCoreEntityExtensionMappings.Configure();
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<VPortalDbContext>(options =>
    {
      /* Remove "includeAllEntities: true" to create
       * default repositories only for aggregate roots */
      options.AddDefaultRepositories(includeAllEntities: true);
    });

    context.Services.AddAbpDbContext<VPortalTenantDbContext>(options =>
    {
      /* Remove "includeAllEntities: true" to create
       * default repositories only for aggregate roots */
      options.AddDefaultRepositories(includeAllEntities: true);
    });

    var configuration = context.Services.GetConfiguration();
    Configure<AbpDbContextOptions>(options =>
    {
      options.UseSqlServer(
              opt => opt.UseCompatibilityLevel(configuration.GetValue("SqlServer:CompatibilityLevel", 120)));
    });
  }
}
