using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Helpers;
using Common.Module.ValueObjects;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.TenantManagement.Module.EventServices
{
  public class ResetTenantAdminPassworEventService :
      IDistributedEventHandler<ResetTenantAdminPasswordMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<ResetTenantAdminPassworEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly TenantDomainService tenantDomainService;

    public ResetTenantAdminPassworEventService(
        IVportalLogger<ResetTenantAdminPassworEventService> logger,
        IResponseContext responseContext,
        TenantDomainService tenantDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.tenantDomainService = tenantDomainService;
    }

    public async Task HandleEventAsync(ResetTenantAdminPasswordMsg eventData)
    {
      if (eventData.ObjectType != "Account")
      {
        return;
      }

      var response = new AccountTenantActionMsg();
      try
      {
        var accountValueObject = eventData.AccountXml.DeserializeFromXml<AccountValueObject>();
        response.AccountId = accountValueObject.Id;
        response.AccountTenantId = accountValueObject.AccountTenantId;

        await tenantDomainService.ResetAdminPassword(accountValueObject.AccountTenantId.Value, accountValueObject.AdminPassword);
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
