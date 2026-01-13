using Volo.Abp.AspNetCore.Mvc;
using VPortal.JobScheduler.Module.Localization;

namespace VPortal.JobScheduler.Module;

public abstract class JobSchedulerModuleController : AbpControllerBase
{
  protected JobSchedulerModuleController()
  {
    LocalizationResource = typeof(JobSchedulerModuleResource);
  }
}
