using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using VPortal.JobScheduler.Module.Localization;

namespace VPortal.JobScheduler.Module;

[DependsOn(
    typeof(AbpValidationModule)
)]
public class JobSchedulerDomainSharedModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<JobSchedulerDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<JobSchedulerModuleResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/JobScheduler");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("JobScheduler", typeof(JobSchedulerModuleResource));
    });
  }
}
