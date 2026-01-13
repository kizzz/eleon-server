using Authorization.Module;
using Infrastructure.Module.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.Infrastructure.Module;

[DependsOn(
    // typeof(AuthorizationModule), // Temporarily commented out to avoid type conflicts
    typeof(AbpValidationModule))]
public class InfrastructureDomainSharedModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<InfrastructureDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<InfrastructureResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/InfrastructureModule");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("InfrastructureModule", typeof(InfrastructureResource));
    });
  }
}
