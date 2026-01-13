using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;

namespace VPortal.Lifecycle.Feature.Module.Templates
{
  public interface IStateTemplateAppService : IApplicationService
  {
    public Task<List<StateTemplateDto>> GetAllAsync(Guid groupId);
    public Task<bool> UpdateOrderIndex(UpdateOrderIndexDto update);
    public Task<bool> UpdateOrderIndexes(Dictionary<Guid, int> order);
    public Task<bool> UpdateApprovalType(UpdateApprovalTypeDto update);
    public Task<bool> UpdateName(Guid id, string name);
    public Task<bool> Enable(StateSwitchDto input);
    public Task<bool> Add(StateTemplateDto stateTemplate);
    public Task<bool> Remove(Guid groupId, Guid stateId);
  }
}
