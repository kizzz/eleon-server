
using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Application.Contracts.Triggers;
using JobScheduler.Module.Triggers;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.JobScheduler.Module.Triggers;

namespace VPortal.JobScheduler.Module.Controllers
{
  [Area(JobSchedulerModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = JobSchedulerModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/JobScheduler/Triggers")]
  public class TriggerController : JobSchedulerModuleController, ITriggerAppService
  {
    private readonly ITriggerAppService appService;
    private readonly IVportalLogger<TriggerController> _logger;

    public TriggerController(
        ITriggerAppService appService,
        IVportalLogger<TriggerController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpPost("Add")]
    public async Task<TriggerDto> AddAsync(TriggerDto trigger)
    {

      var response = await appService.AddAsync(trigger);


      return response;
    }

    [HttpPost("SetTriggerIsEnabled")]
    public async Task<bool> SetTriggerIsEnabledAsync(Guid triggerId, bool isEnabled)
    {

      var response = await appService.SetTriggerIsEnabledAsync(triggerId, isEnabled);


      return response;
    }

    [HttpPost("Update")]
    public async Task<TriggerDto> UpdateAsync(TriggerDto trigger)
    {

      var response = await appService.UpdateAsync(trigger);


      return response;
    }

    [HttpDelete("Delete")]
    public async Task<bool> DeleteAsync(Guid id)
    {

      try
      {
        return await appService.DeleteAsync(id);
      }
      finally
      {
      }
    }

    [HttpGet("GetTriggerById")]
    public async Task<TriggerDto> GetByIdAsync(Guid id)
    {

      var response = await appService.GetByIdAsync(id);


      return response;
    }

    [HttpGet("GetList")]
    public async Task<List<TriggerDto>> GetListAsync(TriggerListRequestDto request)
    {

      var response = await appService.GetListAsync(request);


      return response;
    }
  }
}
