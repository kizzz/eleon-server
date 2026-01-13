using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.TenantManagement.Module.EventServices
{
  public class ActivateAccountTenantEventService :
      IDistributedEventHandler<ActivateAccountTenantMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<ActivateAccountTenantEventService> logger;
    private readonly TenantStatusDomainService tenantStatusDomainService;
    private readonly IResponseContext responseContext;

    public ActivateAccountTenantEventService(
        IVportalLogger<ActivateAccountTenantEventService> logger,
        TenantStatusDomainService tenantStatusDomainService,
        IResponseContext responseContext)
    {
      this.logger = logger;
      this.tenantStatusDomainService = tenantStatusDomainService;
      this.responseContext = responseContext;
    }

    public async Task HandleEventAsync(ActivateAccountTenantMsg eventData)
    {
      if (eventData.DocumentObjectType != "Account")
      {
        return;
      }

      var response = new AccountTenantActionMsg
      {
        AccountId = eventData.AccountId,
        AccountTenantId = eventData.TenantId,
      };
      try
      {
        await tenantStatusDomainService.ActivateTenantWithReplication(eventData.AccountTenantId.Value);
      }
      catch (Exception ex)
      {
        response.ErrorMsg = ex.Message;
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
