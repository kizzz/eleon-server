using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VPortal.Accounting.Module.DomainServices;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.Permissions;

namespace VPortal.Accounting.Module.BillingInformations
{
  [Authorize(AccountingPermissions.General)]
  public class BillingInformationAppService : ModuleAppService, IBillingInformationAppService
  {
    private readonly IVportalLogger<BillingInformationAppService> logger;
    private readonly BillingInformationDomainService domainService;

    public BillingInformationAppService(
        IVportalLogger<BillingInformationAppService> logger,
        BillingInformationDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<BillingInformationDto> GetBillingInfoDetailsById(Guid id)
    {
      BillingInformationDto result = null;
      try
      {
        var entity = await domainService.GetBillingInfoDetailsById(id);
        result = ObjectMapper.Map<BillingInformationEntity, BillingInformationDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }
  }
}
