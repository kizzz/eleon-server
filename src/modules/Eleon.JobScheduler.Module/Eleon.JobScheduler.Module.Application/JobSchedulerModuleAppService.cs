using Volo.Abp.Application.Services;
using VPortal.JobScheduler.Module.Localization;

namespace VPortal.JobScheduler.Module;

public abstract class JobSchedulerModuleAppService : ApplicationService
{
  protected JobSchedulerModuleAppService()
  {
    LocalizationResource = typeof(JobSchedulerModuleResource);
    ObjectMapperContext = typeof(JobSchedulerApplicationModule);
  }
}
