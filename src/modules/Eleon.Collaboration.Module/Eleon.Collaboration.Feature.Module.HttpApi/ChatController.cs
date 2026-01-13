using Volo.Abp.AspNetCore.Mvc;
using VPortal.Collaboration.Feature.Module.Localization;

namespace VPortal.Collaboration.Feature.Module;

public abstract class ChatController : AbpControllerBase
{
  protected ChatController()
  {
    LocalizationResource = typeof(CollaborationResource);
  }
}
