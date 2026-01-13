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
  public class StateActorAuditAppService : ModuleAppService, IStateActorAuditAppService
  {
    private readonly StateActorAuditDomainService stateActorAuditDomainService;
    private readonly IVportalLogger<StateActorAuditAppService> logger;

    public StateActorAuditAppService(
        StateActorAuditDomainService stateActorAuditDomainService,
        IVportalLogger<StateActorAuditAppService> logger)
    {
      this.stateActorAuditDomainService = stateActorAuditDomainService;
      this.logger = logger;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Add(StateActorAuditDto stateActorAuditDto)
    {
      bool result = false;
      try
      {
        var stateActorAudit = ObjectMapper
            .Map<StateActorAuditDto, StateActorAuditEntity>(stateActorAuditDto);
        result = await stateActorAuditDomainService.Add(stateActorAudit);
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
        result = await stateActorAuditDomainService.Remove(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
