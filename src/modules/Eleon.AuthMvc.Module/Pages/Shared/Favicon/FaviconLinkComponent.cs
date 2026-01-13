using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace VPortal.Pages.Shared.Favicon
{
  public class FaviconLinkComponent : AbpViewComponent
  {
    public IViewComponentResult Invoke()
    {
      return View("/Pages/Shared/Favicon/Default.cshtml");
    }
  }
}
