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

  public class StateAuditDomainService : DomainService
  {
    private readonly ICurrentTenant currentTenant;
    private readonly IVportalLogger<StateAuditDomainService> logger;
    private readonly IStatesGroupAuditsRepository repository;

    public StateAuditDomainService(
        ICurrentTenant currentTenant,
        IVportalLogger<StateAuditDomainService> logger,
        IStatesGroupAuditsRepository repository)
    {
      this.currentTenant = currentTenant;
      this.logger = logger;
      this.repository = repository;
    }

    public async Task<bool> Add(StateAuditEntity stateTemplate)
    {
      bool result = false;
      try
      {
        var group = await repository.GetAsync(stateTemplate.StatesGroupId);
        await repository.Add(group);
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
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
        await repository.DeleteAsync(id, true);
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }
  }
}
