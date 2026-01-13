using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions.Constants;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Uow.EntityFrameworkCore;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Repositories;

namespace VPortal.TenantManagement.Module.EntityFrameworkCore;

[DependsOn(
    typeof(TenantManagementDomainModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class TenantManagementEntityFrameworkCoreModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    TenantManagementEfCoreEntityExtensionMappings.Configure();
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<TenantManagementDbContext>(options =>
    {
      options.AddRepository<TenantSettingEntity, TenantSettingRepository>();
      //options.AddRepository<IdentityUser, CommonUsersRepository>();
    });

    context.Services.AddTransient<ITenantRepository, CustomEfCoreTenantRepository>();

    Configure<PermissionManagementOptions>(options =>
    {
      options.IsDynamicPermissionStoreEnabled = true;
    });

    Configure<FeatureManagementOptions>(options =>
    {
      options.IsDynamicFeatureStoreEnabled = true;
    });

    ObjectExtensionManager.Instance
        .AddOrUpdateProperty<FeatureGroupDefinitionRecord, string>(PermissionConstants.SourceIdPropertyName);
    ObjectExtensionManager.Instance
        .AddOrUpdateProperty<FeatureDefinitionRecord, string>(PermissionConstants.SourceIdPropertyName);

    ObjectExtensionManager.Instance
        .AddOrUpdateProperty<PermissionGroupDefinitionRecord, string>(PermissionConstants.SourceIdPropertyName);
    ObjectExtensionManager.Instance
        .AddOrUpdateProperty<PermissionGroupDefinitionRecord, string>("FeatureName");

    ObjectExtensionManager.Instance
        .AddOrUpdateProperty<PermissionDefinitionRecord, string>(PermissionConstants.SourceIdPropertyName);
    ObjectExtensionManager.Instance
        .AddOrUpdateProperty<PermissionDefinitionRecord, string>("FeatureName");
    ObjectExtensionManager.Instance
        .AddOrUpdateProperty<PermissionDefinitionRecord, int>("Order");
  }
}
