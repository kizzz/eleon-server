using Logging.Module;
using Microsoft.Extensions.Logging;
using Sentry;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Repositories.Audits;

namespace VPortal.Lifecycle.Feature.Module.DomainServices
{

  public class StateActorAuditDomainService : DomainService
  {
    private readonly ICurrentTenant currentTenant;
    private readonly IVportalLogger<StateActorAuditDomainService> _logger;
    private readonly IStatesGroupAuditsRepository repository;

    public StateActorAuditDomainService(
        ICurrentTenant currentTenant,
        IVportalLogger<StateActorAuditDomainService> logger,
        IStatesGroupAuditsRepository repository)
    {
      this.currentTenant = currentTenant;
      _logger = logger;
      this.repository = repository;
    }

    public async Task<bool> Add(StateActorAuditEntity stateActorAudit)
    {
      bool result = false;
      try
      {
        var state = await repository.GetState(stateActorAudit.StateId);
        state.Actors.Add(stateActorAudit);
        result = true;
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> Remove(Guid id)
    {
      bool result = false;
      try
      {
        var actor = await repository.GetStateActor(id);
        var state = actor.State;
        state.Actors.Remove(actor);
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }
  }
}
