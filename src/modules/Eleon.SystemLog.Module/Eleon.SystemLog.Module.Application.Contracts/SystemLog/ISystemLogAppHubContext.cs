
using AutoMapper.Internal.Mappers;

namespace VPortal.DocMessageLog.Module.DocMessageLogs;
public interface ISystemLogAppHubContext
{
  Task PushSystemLogAsync(List<Guid> targetUsers, SystemLogDto logDto);
}
