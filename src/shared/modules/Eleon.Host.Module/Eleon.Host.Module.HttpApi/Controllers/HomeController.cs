using Eleon.Host.Module.Eleoncore.Host.Module.HttpApi;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace VPortal.Controllers;

[Area(HostRemoteServiceConsts.ModuleName)]
[RemoteService(Name = HostRemoteServiceConsts.RemoteServiceName)]
public class HomeController : AbpController
{
  public ActionResult Index()
  {
    return Redirect("~/swagger");
  }

  [HttpGet("profiler")]
  public ActionResult MiniProfiler()
  {
    return View("~/Components/MiniProfiler/Default.cshtml");
  }
}
