using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Application.Contracts.Triggers;
using JobScheduler.Module.Triggers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.JobScheduler.Module.Triggers
{
  public interface ITriggerAppService : IApplicationService
  {
    Task<List<TriggerDto>> GetListAsync(TriggerListRequestDto request);
    Task<TriggerDto> GetByIdAsync(Guid id);

    Task<bool> SetTriggerIsEnabledAsync(Guid triggerId, bool isEnabled);

    Task<TriggerDto> AddAsync(TriggerDto trigger);
    Task<TriggerDto> UpdateAsync(TriggerDto trigger);
    Task<bool> DeleteAsync(Guid id);
  }
}
