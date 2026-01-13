using Authorization.Module.MicroserviceInitialization;
using Authorization.Module.Permissions;
using EleonsoftAbp.EleonsoftPermissions;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions;
using ModuleCollector.Identity.Module.Identity.Module.Domain.ApiKey;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Shared.Constants;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.AutoMapper;
using Volo.Abp.Domain;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.TenantManagement;
using VPortal.Identity.Module.Permissions;
using VPortal.Infrastructure.Module.Domain;
using VPortal.TenantManagement.Module.Permissions;

namespace VPortal.Infrastructure.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpSettingsModule),
    typeof(AbpPermissionManagementDomainModule),
    typeof(InfrastructureDomainSharedModule))]
public class MinimalInfrastructureDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    base.ConfigureServices(context);
    context.Services.AddAutoMapperObjectMapper<MinimalInfrastructureDomainModule>();

    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddProfile<ModuleDomainAutoMapperProfile>(validate: true);
    });
  }
}

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpTenantManagementDomainModule),
    typeof(InfrastructureDomainSharedModule)
)]
public class InfrastructureDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    base.ConfigureServices(context);
    context.Services.AddAutoMapperObjectMapper<InfrastructureDomainModule>();

    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddProfile<ModuleDomainAutoMapperProfile>(validate: true);
    });

    context.Services.AddHttpClient();



    Configure<AbpPermissionOptions>(options =>
    {
      options.ValueProviders.Remove<RolePermissionValueProvider>();
      options.ValueProviders.Add<VPortalRolePermissionValueProvider>();
      options.ValueProviders.Remove<UserPermissionValueProvider>();
      options.ValueProviders.Add<VportalUserPermissionValueProvider>();
      options.ValueProviders.Add<ApiKeyPermissionValueProvider>();
    });

    context.Services.AddHostedService<MicroserviceInitializationHostedService>();

    Configure<PermissionManagementOptions>(options =>
    {
      options.ProviderPolicies.Add(ApiKeyConstants.ApiKeyPermissionProviderName, IdentityPermissions.ApiKey.Default);
      options.ManagementProviders.Add<ApiKeyPermissionManagementProvider>();

      options.ManagementProviders.Remove<RolePermissionManagementProvider>();
      options.ManagementProviders.Remove<UserPermissionManagementProvider>();

      options.ManagementProviders.Add<VportalRolePermissionManagementProvider>();
      options.ManagementProviders.Add<VportalUserPermissionManagementProvider>();
    });

    context.Services.AddEleonsoftPermissions().ForbidApiKeysWithoutAuthorize();
  }
}
