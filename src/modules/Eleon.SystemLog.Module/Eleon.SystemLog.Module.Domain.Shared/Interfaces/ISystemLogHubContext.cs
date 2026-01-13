
using VPortal.DocMessageLog.Module.DocMessageLogs;
using VPortal.DocMessageLog.Module.Entities;

namespace VPortal.DocMessageLog.Module.Domain
{
  public interface ISystemLogHubContext
  {
    Task PushSystemLogAsync(List<Guid> targetUsers, SystemLogEntity logEntity);
  }
}
