using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;

namespace VPortal.Lifecycle.Feature.Module.Templates
{
  public interface IStateActorTemplateAppService : IApplicationService
  {
    public Task<List<StateActorTemplateDto>> GetAllAsync(Guid stateId);
    public Task<bool> Add(StateActorTemplateDto stateActorTemplate);
    public Task<bool> Remove(Guid id);
    public Task<bool> Enable(StateSwitchDto stateSwitchDto);
    public Task<bool> Update(StateActorTemplateDto stateActorTemplate);
    public Task<bool> UpdateOrderIndexes(Dictionary<Guid, int> order);
  }
}
