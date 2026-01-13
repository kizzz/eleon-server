using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using VPortal.HealthCheckModule.Module.Localization;

namespace VPortal.HealthCheckModule.Module;

[DependsOn(
    typeof(AbpValidationModule),
    typeof(AbpLocalizationModule)
)]
public class HealthCheckDomainSharedModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<HealthCheckDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<HealthCheckModuleResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/HealthCheckModule");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("HealthCheckModule", typeof(HealthCheckModuleResource));
    });

    Configure<HealthCheckModuleOptions>(context.Configuration.GetSection("HealthCheckModule"));
  }
}
