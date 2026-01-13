using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.Identity.Module.EventServices
{
  public class GetTenantsEventService :
      IDistributedEventHandler<GetAllTenantsMsg>,
      ITransientDependency
  {
    private readonly IResponseContext responseContext;
    private readonly IVportalLogger<GetTenantsEventService> logger;
    private readonly TenantDomainService tenantDomainService;

    public GetTenantsEventService(
        IResponseContext responseContext,
        IVportalLogger<GetTenantsEventService> logger,
        TenantDomainService tenantDomainService)
    {
      this.responseContext = responseContext;
      this.logger = logger;
      this.tenantDomainService = tenantDomainService;
    }

    public async Task HandleEventAsync(GetAllTenantsMsg eventData)
    {
      var response = new AllTenantsGotMsg();
      try
      {
        var tenants = await tenantDomainService.GetListAsync();
        response.Tenants = tenants.Select(t => new TenantEto()
        {
          Id = t.Id,
          Name = t.Name,
          ParentId = t.ExtraProperties.GetValueOrDefault("ParentId")?.ToString(),
          IsRoot = t.ExtraProperties.GetValueOrDefault("IsRoot")?.ToString() == "true",
          Status = GetTenantStatus(t.GetProperty("Status", "Active")), // tenantSettings.FirstOrDefault(s => s.TenantId == t.Id)?.Status ?? TenantStatus.Active,
          ConnectionStrings = t.ConnectionStrings.Select(x => new TenantConnectionStringEto { Name = x.Name, Value = x.Value }).ToList(),
        }).ToList();
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

    private TenantStatus GetTenantStatus(string status)
    {
      return status?.ToLower() switch
      {
        "active" => TenantStatus.Active,
        "terminated" => TenantStatus.Terminated,
        "suspended" => TenantStatus.Suspended,
        _ => TenantStatus.Active,
      };
    }
  }
}
