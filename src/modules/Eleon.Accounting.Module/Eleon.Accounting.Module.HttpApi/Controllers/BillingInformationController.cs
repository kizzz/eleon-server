using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Accounting.Module.BillingInformations;

namespace VPortal.Accounting.Module.Controllers
{
  [Area(AccountingRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = AccountingRemoteServiceConsts.RemoteServiceName)]
  [Route("api/account/billingInformations")]
  public class BillingInformationController : ModuleController, IBillingInformationAppService
  {
    private readonly IBillingInformationAppService appService;
    private readonly IVportalLogger<BillingInformationController> logger;

    public BillingInformationController(
        IVportalLogger<BillingInformationController> logger,
        IBillingInformationAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpPost("GetBillingInfoDetailsById")]
    public async Task<BillingInformationDto> GetBillingInfoDetailsById(Guid id)
    {

      var response = await appService.GetBillingInfoDetailsById(id);


      return response;
    }
  }
}
