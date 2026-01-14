using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Eleon.SystemServices.Module.Full.Eleon.SystemServicesModule.Module.Application.Contracts.SystemLog;

public interface ISystemLogAppService : IApplicationService
{
  public Task<bool> WriteManyAsync(List<CreateSystemLogDto> logs);
}
