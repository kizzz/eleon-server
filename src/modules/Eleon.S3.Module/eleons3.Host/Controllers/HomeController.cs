using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace VPortal.Controllers;

[RemoteService(IsEnabled = true, Name="Eleoncore")]
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
