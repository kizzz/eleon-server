using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using VPortal.Accounting.Module.DomainServices;
using VPortal.Accounting.Module.Permissions;

namespace VPortal.Accounting.Module.Invoices
{
  [Authorize(AccountingPermissions.General)]
  public class InvoiceAppService : ModuleAppService, IInvoiceAppService
  {
    private readonly IVportalLogger<InvoiceAppService> logger;
    private readonly InvoiceDomainService domainService;

    public InvoiceAppService(
        IVportalLogger<InvoiceAppService> logger,
        InvoiceDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }
  }
}
