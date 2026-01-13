using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Application.Contracts.Actions;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.JobScheduler.Module.Actions;

namespace VPortal.JobScheduler.Module.Controllers
{
  [Area(JobSchedulerModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = JobSchedulerModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/JobScheduler/Actions")]
  public class ActionController : JobSchedulerModuleController, IActionAppService
  {
    private readonly IActionAppService appService;
    private readonly IVportalLogger<ActionController> _logger;

    public ActionController(
        IActionAppService appService,
        IVportalLogger<ActionController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpPost("Add")]
    public async Task<ActionDto> AddAsync(ActionDto action)
    {

      try
      {
        return await appService.AddAsync(action);
      }
      finally
      {
      }
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

    [HttpGet("GetById")]
    public async Task<ActionDto> GetByIdAsync(Guid id)
    {

      try
      {
        return await appService.GetByIdAsync(id);
      }
      finally
      {
      }
    }

    [HttpGet("GetList")]
    public async Task<List<ActionDto>> GetListAsync(ActionListRequestDto request)
    {

      try
      {
        var response = await appService.GetListAsync(request);
        return response;
      }
      finally
      {
      }
    }

    [HttpPut("Update")]
    public async Task<ActionDto> UpdateAsync(ActionDto action)
    {

      try
      {
        return await appService.UpdateAsync(action);
      }
      finally
      {
      }
    }
  }
}
