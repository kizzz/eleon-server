using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;

namespace VPortal.Lifecycle.Feature.Module.Templates
{
  public interface IStatesGroupTemplateAppService : IApplicationService
  {
    public Task<FullStatesGroupTemplateDto> GetAsync(Guid id);
    public Task<PagedResultDto<StatesGroupTemplateDto>> GetListAsync(GetStatesGroupsDto input);
    public Task<bool> Add(StatesGroupTemplateDto statesGroupTemplate);
    public Task<bool> Enable(StatesGroupSwitchDto groupSwitchDto);
    public Task<bool> Rename(Guid id, string newName);
    public Task<bool> Remove(Guid id);
    public Task<bool> Update(StatesGroupTemplateDto statesGroupTemplate);
  }
}
