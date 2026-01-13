using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.Lifecycle.Feature.Module.Dto.Audits;

namespace VPortal.Lifecycle.Feature.Module.Audits
{
  public interface IStateAuditAppService : IApplicationService
  {
    public Task<bool> Add(StateAuditDto stateAudit);
    public Task<bool> Remove(Guid id);
  }
}
