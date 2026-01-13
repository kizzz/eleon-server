using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using VPortal.Accounting.Module.Invoices;

namespace VPortal.Accounting.Module.Controllers
{
  [Area(AccountingRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = AccountingRemoteServiceConsts.RemoteServiceName)]
  [Route("api/account/invoices")]
  public class InvoiceController : ModuleController, IInvoiceAppService
  {
    private readonly IInvoiceAppService appService;
    private readonly IVportalLogger<InvoiceController> logger;

    public InvoiceController(
        IVportalLogger<InvoiceController> logger,
        IInvoiceAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }
  }
}
