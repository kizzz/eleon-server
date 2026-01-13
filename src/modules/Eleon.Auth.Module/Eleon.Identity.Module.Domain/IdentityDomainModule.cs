using Common.Module.Constants;
using EleonsoftAbp.EleonsoftPermissions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Shared.Constants;
using TenantSettings.Module;
using TenantSettings.Module.Cache;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;
using Volo.Abp.Reflection;
using VPortal.Identity.Module.IdentityServerExtensions.Claims;
using VPortal.Identity.Module.Sessions;

namespace VPortal.Identity.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(IdentityDomainSharedModule),
    typeof(AbpIdentityServerDomainModule),
    typeof(TenantSettingsModule)
)]
public class IdentityDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<VPortalIdentityClaimsOptions>(opt =>
    {
      opt.AdditionalOptionalClaimTypes.AddRange(ReflectionHelper.GetPublicConstantsRecursively(typeof(OptionalUserClaims)));
      opt.AdditionalOptionalClaimTypes.Add(VPortalExtensionGrantsConsts.MachineKey.MachineKeyClaim);
      opt.AdditionalOptionalClaimTypes.Add(VPortalExtensionGrantsConsts.ApiKey.ApiKeyTypeClaim);
      opt.AdditionalOptionalClaimTypes.Add(VPortalExtensionGrantsConsts.Impresonation.ImpersonatorUserClaim);
      opt.AdditionalOptionalClaimTypes.Add(VPortalExtensionGrantsConsts.Impresonation.ImpersonatorTenantClaim);
      opt.AdditionalOptionalClaimTypes.Add(PeriodicPasswordChangeConsts.PasswordChangeRequiredClaimType);
    });

    Configure<TenantSettingsCacheOptions>(opt =>
    {
      opt.NotifyIdentityUrlsChange = true;
    });

    context.Services.Replace(ServiceDescriptor.Transient<IClaimsService, VPortalIdentityClaimsService>());
    context.Services.Replace(ServiceDescriptor.Scoped<IPersistedGrantStore, CustomPersistedGrantStore>());

    context.Services.AddEleonsoftPermissions().ForbidApiKeysWithoutAuthorize();
  }
}
