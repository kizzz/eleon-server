using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Application.Contracts.Actions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.JobScheduler.Module.Actions
{
  public interface IActionAppService : IApplicationService
  {
    Task<ActionDto> GetByIdAsync(Guid id);
    Task<List<ActionDto>> GetListAsync(ActionListRequestDto request);
    Task<ActionDto> AddAsync(ActionDto action);
    Task<ActionDto> UpdateAsync(ActionDto action);
    Task<bool> DeleteAsync(Guid id);
  }
}
