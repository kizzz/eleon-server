using BackgroundJobs.Module.BackgroundJobs;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.BackgroundJobs.Module;

namespace BackgroundJobs.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/BackgroundJob/BackgroundJobs")]
  public class BackgroundJobController : ModuleController, IBackgroundJobAppService
  {
    private readonly IBackgroundJobAppService appService;
    private readonly IVportalLogger<BackgroundJobController> _logger;

    public BackgroundJobController(
        IBackgroundJobAppService appService,
        IVportalLogger<BackgroundJobController> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpPost("CancelBackgroundJob")]
    public async Task<bool> CancelBackgroundJobAsync(Guid id)
    {

      try
      {
        return await appService.CancelBackgroundJobAsync(id);
      }
      finally
      {
      }
    }

    [HttpPost("Complete")]
    public async Task<BackgroundJobDto> CompleteAsync(BackgroundJobExecutionCompleteDto input)
    {

      var response = await appService.CompleteAsync(input);


      return response;
    }

    [HttpPost("Create")]
    public async Task<BackgroundJobDto> CreateAsync(CreateBackgroundJobDto input)
    {

      var response = await appService.CreateAsync(input);


      return response;
    }

    [HttpGet("GetBackgroundJobById")]
    public async Task<FullBackgroundJobDto> GetBackgroundJobByIdAsync(Guid id)
    {

      var response = await appService.GetBackgroundJobByIdAsync(id);


      return response;
    }

    [HttpGet("GetBackgroundJobList")]
    public async Task<PagedResultDto<BackgroundJobHeaderDto>> GetBackgroundJobListAsync(BackgroundJobListRequestDto input)
    {

      var response = await appService.GetBackgroundJobListAsync(input);


      return response;
    }

    [HttpPost("MarkExecutionStarted")]
    public Task<bool> MarkExecutionStartedAsync(Guid jobId, Guid executionId)
    {

      try
      {
        return appService.MarkExecutionStartedAsync(jobId, executionId);
      }
      finally
      {
      }
    }

    [HttpPost("RetryBackgroundJob")]
    public async Task<bool> RetryBackgroundJobAsync(Guid id)
    {

      var response = await appService.RetryBackgroundJobAsync(id);


      return response;
    }
  }
}
