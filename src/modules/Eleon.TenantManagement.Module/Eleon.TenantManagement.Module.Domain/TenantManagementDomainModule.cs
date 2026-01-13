using Authorization.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides;
using SharedModule.modules.MultiTenancy.Module;
using TenantSettings.Module;
using Volo.Abp.AuditLogging;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace VPortal.TenantManagement.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(TenantManagementDomainSharedModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpTenantManagementDomainModule),
    typeof(AbpEmailingModule),
    typeof(BlobStoringDatabaseDomainModule),
    typeof(AbpAutoMapperModule),
    typeof(TenantSettingsModule),
    typeof(IdentityQueryingDomainModule)
)]
public class TenantManagementDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    var configuration = context.Services.GetConfiguration();
    context.Services.AddAutoMapperObjectMapper<TenantManagementDomainModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<TenantManagementDomainModule>(validate: true);
    });
    Configure<PermissionManagementOptions>(options =>
    {
      options.SaveStaticPermissionsToDatabase = true;
      options.IsDynamicPermissionStoreEnabled = true;
    });

    Configure<FeatureManagementOptions>(options =>
    {
      options.SaveStaticFeaturesToDatabase = true;
      options.IsDynamicFeatureStoreEnabled = true;
    });

    Configure<AbpMultiTenancyOptions>(options =>
    {
      options.IsEnabled = configuration.GetValue($"{EleonMultiTenancyOptions.DefaultSectionName}:{nameof(EleonMultiTenancyOptions.Enabled)}", true);
    });

    context.Services.AddTransient<IStaticPermissionDefinitionStore, CustomStaticPermissionDefinitionStore>();
    context.Services.AddTransient<IPermissionDefinitionManager, CustomPermissionDefinitionManager>();
    context.Services.AddTransient<IStaticPermissionSaver, CustomStaticSaver>();
    context.Services.AddTransient<IDynamicPermissionDefinitionStore, CustomDynamicPermissionDefinitionStore>();
    context.Services.AddTransient<IPermissionManager, CustomPermissionManager>();
    context.Services.AddTransient<IDynamicFeatureDefinitionStore, CustomDynamicFeatureDefinitionStore>();
    context.Services.AddTransient<IFeatureDefinitionManager, CustomFeatureDefinitionManager>();
  }
}
