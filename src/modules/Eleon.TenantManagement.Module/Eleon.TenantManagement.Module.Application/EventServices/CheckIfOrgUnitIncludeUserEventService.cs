using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.TenantManagement.Module.EventServices
{
  public class CheckIfOrgUnitIncludeUserEventService :
      IDistributedEventHandler<CheckIfOrgUnitIncludeUserMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CheckIfOrgUnitIncludeUserEventService> logger;
    private readonly OrganizationUnitDomainService organizationUnitDomainService;
    private readonly IResponseContext responseContext;

    public CheckIfOrgUnitIncludeUserEventService(
        IVportalLogger<CheckIfOrgUnitIncludeUserEventService> logger,
        OrganizationUnitDomainService organizationUnitDomainService,
        IResponseContext responseContext)
    {
      this.logger = logger;
      this.organizationUnitDomainService = organizationUnitDomainService;
      this.responseContext = responseContext;
    }

    public async Task HandleEventAsync(CheckIfOrgUnitIncludeUserMsg eventData)
    {
      var response = new ActionCompletedMsg();
      try
      {
        response.Success = await organizationUnitDomainService.CheckIfOrgUnitIncludeUser(eventData.OrgUnitId, eventData.UserId);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
