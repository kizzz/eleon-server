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
  public class SuspendAccountTenantEventService :
      IDistributedEventHandler<SuspendAccountTenantMsg>,
      ITransientDependency
  {
    private readonly IResponseContext responseContext;
    private readonly IVportalLogger<SuspendAccountTenantEventService> logger;
    private readonly TenantStatusDomainService tenantStatusDomainService;

    public SuspendAccountTenantEventService(
        IResponseContext responseContext,
        IVportalLogger<SuspendAccountTenantEventService> logger, TenantStatusDomainService tenantStatusDomainService)
    {
      this.responseContext = responseContext;
      this.logger = logger;
      this.tenantStatusDomainService = tenantStatusDomainService;
    }

    public async Task HandleEventAsync(SuspendAccountTenantMsg eventData)
    {
      if (eventData.ObjectType != "Account")
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
        await tenantStatusDomainService.SuspendTenantWithReplication(eventData.AccountTenantId.Value);
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
