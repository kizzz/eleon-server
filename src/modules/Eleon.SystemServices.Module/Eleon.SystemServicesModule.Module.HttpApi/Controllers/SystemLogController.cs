using Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.Application.Contracts.SystemLog;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using VPortal.SystemServicesModule.Module;

namespace Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.HttpApi.Controllers;

[Area(SystemServicesRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SystemServicesRemoteServiceConsts.RemoteServiceName)]
[Route("api/system-services/system-log")]
public class SystemLogController : SystemServicesModuleController, ISystemLogAppService
{
  private readonly ISystemLogAppService _appService;

  public SystemLogController(ISystemLogAppService appService)
  {
    _appService = appService;
  }

  [HttpPost("WriteMany")]
  public async Task<bool> WriteManyAsync(List<CreateSystemLogDto> logs)
  {
    return await _appService.WriteManyAsync(logs);
  }
}
