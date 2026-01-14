using Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.Application.Contracts.SystemLog;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.EventBus.Distributed;
using VPortal.SystemServicesModule.Module;

namespace Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.Application.SystemLog;

[Authorize]
public class SystemLogAppService : SystemServicesModuleAppService, ISystemLogAppService
{
  private readonly IDistributedEventBus _eventBus;

  public SystemLogAppService(IDistributedEventBus eventBus)
  {
    _eventBus = eventBus;
  }

  public async Task<bool> WriteManyAsync(List<CreateSystemLogDto> logs)
  {
    // todo: write logs
    return true;
  }
}
