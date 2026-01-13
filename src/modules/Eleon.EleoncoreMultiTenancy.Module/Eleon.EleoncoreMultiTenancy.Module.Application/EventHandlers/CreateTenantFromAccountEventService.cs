using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.TenantManagement.Module.EventServices
{
  public class CreateTenantFromAccountEventService :
      IDistributedEventHandler<CreateTenantFromAccountMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CreateTenantFromAccountEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly TenantDomainService tenantDomainService;

    public CreateTenantFromAccountEventService(
        IVportalLogger<CreateTenantFromAccountEventService> logger,
        IResponseContext responseContext,
        TenantDomainService tenantDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.tenantDomainService = tenantDomainService;
    }

    public async Task HandleEventAsync(CreateTenantFromAccountMsg eventData)
    {
      if (eventData.ObjectType != "Account")
      {
        return;
      }

      var message = new SendNewAccountTenantMsg();
      try
      {
        var result = await tenantDomainService.CreateTenant(
            eventData.TenantName,
            eventData.AdminEmail,
            eventData.AdminPassword,
            eventData.CreateDatabase,
            eventData.NewDatabaseName,
            eventData.NewUserName,
            eventData.NewUserPassword,
            eventData.DefaultConnectionString,
            eventData.IsRoot);

        message.NewAccountTenantId = result.TenantId;
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(message);
      }

    }
  }
}
