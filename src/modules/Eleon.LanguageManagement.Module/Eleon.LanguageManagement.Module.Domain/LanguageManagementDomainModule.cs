using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using VPortal.LanguageManagement.Module;
using VPortal.LanguageManagement.Module.LocalizationResources;

namespace VPortal.LanguageManagement;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(LanguageManagementDomainSharedModule),
    typeof(AbpSettingManagementDomainModule)
)]
public class LanguageManagementDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(opt =>
    {
      opt.GlobalContributors.Add<LocalizationResourceContributor>();
    });
  }
}
