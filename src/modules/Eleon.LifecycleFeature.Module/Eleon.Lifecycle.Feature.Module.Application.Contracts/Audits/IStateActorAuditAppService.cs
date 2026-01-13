using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.Lifecycle.Feature.Module.Dto.Audits;

namespace VPortal.Lifecycle.Feature.Module.Audits
{
  public interface IStateActorAuditAppService : IApplicationService
  {
    public Task<bool> Add(StateActorAuditDto stateActorAudit);
    public Task<bool> Remove(Guid id);
  }
}
