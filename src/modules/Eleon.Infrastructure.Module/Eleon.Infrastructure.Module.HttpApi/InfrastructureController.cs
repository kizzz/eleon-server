using Infrastructure.Module.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace VPortal.Infrastructure.Module;

public abstract class InfrastructureController : AbpControllerBase
{
  protected InfrastructureController()
  {
    LocalizationResource = typeof(InfrastructureResource);
  }
}
