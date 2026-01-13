using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Dto.Audits;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Permissions;

namespace VPortal.Lifecycle.Feature.Module.Audits
{
  [Authorize(LifecyclePermissions.LifecycleManager)]
  public class StateAuditAppService : ModuleAppService, IStateAuditAppService
  {
    private readonly StateAuditDomainService stateAuditDomainService;
    private readonly IVportalLogger<StateAuditAppService> logger;

    public StateAuditAppService(
        StateAuditDomainService stateAuditDomainService,
        IVportalLogger<StateAuditAppService> logger)
    {
      this.stateAuditDomainService = stateAuditDomainService;
      this.logger = logger;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Add(StateAuditDto stateAuditDto)
    {
      bool result = false;
      try
      {
        var stateAudit = ObjectMapper
            .Map<StateAuditDto, StateAuditEntity>(stateAuditDto);
        result = await stateAuditDomainService.Add(stateAudit);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Remove(Guid id)
    {
      bool result = false;
      try
      {
        result = await stateAuditDomainService.Remove(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
