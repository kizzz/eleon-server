using Logging.Module;

using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using VPortal.DocMessageLog.Module.DocMessageLogs;
using VPortal.DocMessageLog.Module.Domain;
using VPortal.DocMessageLog.Module.Entities;

namespace VPortal.DocMessageLog.Module.DocMessageLogs
{
  public class SystemLogHubContext : ISystemLogHubContext, ITransientDependency
  {
    private readonly IVportalLogger<SystemLogHubContext> logger;
    private readonly ISystemLogAppHubContext hubContext;
    private readonly IObjectMapper objectMapper;

    public SystemLogHubContext(IVportalLogger<SystemLogHubContext> logger, ISystemLogAppHubContext hubContext, IObjectMapper objectMapper)
    {
      this.logger = logger;
      this.hubContext = hubContext;
      this.objectMapper = objectMapper;
    }

    public async Task PushSystemLogAsync(List<Guid> targetUsers, SystemLogEntity logEntity)
    {
      try
      {
        var mapped = objectMapper.Map<SystemLogEntity, SystemLogDto>(logEntity);
        await hubContext.PushSystemLogAsync(targetUsers, mapped);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }
  }
}
