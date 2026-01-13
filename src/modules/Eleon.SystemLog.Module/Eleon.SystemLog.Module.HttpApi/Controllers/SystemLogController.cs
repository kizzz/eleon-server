using EleonsoftModuleCollector.SystemLog.Module.SystemLog.Module.Application.Contracts.SystemLog;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.DocMessageLog.Module.DocMessageLogs;

namespace VPortal.DocMessageLog.Module.Controllers;

[Area(SystemLogModuleRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SystemLogModuleRemoteServiceConsts.RemoteServiceName)]
[Route("api/SystemLog/SystemLog")]
public class SystemLogController : SystemLogModuleController, ISystemLogAppService
{
  private readonly IVportalLogger<SystemLogController> _logger;
  private readonly ISystemLogAppService _docMsgLogAppService;

  public SystemLogController(
      IVportalLogger<SystemLogController> logger,
      ISystemLogAppService docMsgLogAppService)
  {
    _logger = logger;
    _docMsgLogAppService = docMsgLogAppService;
  }

  [HttpGet("GetById")]
  public async Task<FullSystemLogDto> GetByIdAsync(Guid id)
  {

    var response = await _docMsgLogAppService.GetByIdAsync(id);


    return response;
  }

  [HttpGet("GetList")]
  public async Task<PagedResultDto<SystemLogDto>> GetListAsync(SystemLogListRequestDto input)
  {

    var response = await _docMsgLogAppService.GetListAsync(input);


    return response;
  }

  [HttpPost("MarkReaded")]
  public async Task<bool> MarkReadedAsync(MarkSystemLogsReadedRequestDto request)
  {
    try
    {
      return await _docMsgLogAppService.MarkReadedAsync(request);
    }
    finally
    {
    }
  }

  [HttpPost("MarkAllReaded")]
  public async Task MarkAllReadedAsync()
  {
    try
    {
      await _docMsgLogAppService.MarkAllReadedAsync();
    }
    finally
    {
    }
  }



  [HttpPost("Write")]
  public async Task<bool> WriteAsync(CreateSystemLogDto request)
  {
    try
    {
      return await _docMsgLogAppService.WriteAsync(request);
    }
    finally
    {
    }
  }

  [HttpPost("WriteMany")]
  public async Task<bool> WriteManyAsync(List<CreateSystemLogDto> request)
  {
    try
    {
      return await _docMsgLogAppService.WriteManyAsync(request);
    }
    finally
    {
    }
  }

  [HttpGet("GetTotalUnresolvedCount")]
  public async Task<UnresolvedSystemLogCountDto> GetTotalUnresolvedCountAsync()
  {
    try
    {
      return await _docMsgLogAppService.GetTotalUnresolvedCountAsync();
    }
    finally
    {
    }
  }
}
