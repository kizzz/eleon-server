using EleonsoftModuleCollector.SystemLog.Module.SystemLog.Module.Application.Contracts.SystemLog;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace VPortal.DocMessageLog.Module.DocMessageLogs;

public interface ISystemLogAppService : IApplicationService
{
  Task<FullSystemLogDto> GetByIdAsync(Guid id);
  Task<PagedResultDto<SystemLogDto>> GetListAsync(SystemLogListRequestDto input);
  Task<bool> MarkReadedAsync(MarkSystemLogsReadedRequestDto request);
  Task<bool> WriteAsync(CreateSystemLogDto request);
  Task<bool> WriteManyAsync(List<CreateSystemLogDto> request);
  Task MarkAllReadedAsync();
  Task<UnresolvedSystemLogCountDto> GetTotalUnresolvedCountAsync();
}
