using Eleon.Notificator.Module.Eleon.Notificator.Module.Domain.Shared.Services;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.PushNotifications;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.System;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Jobs;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.ValueObjects;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using VPortal.Notificator.Module.Auditor;

namespace Eleon.Notificator.Module.Eleon.Notificator.Module.Application.SystemMessages;
public class SystemHubContext : ISystemHubContext, ITransientDependency
{
  private readonly IVportalLogger<SystemHubContext> logger;
  private readonly ISystemAppHubContext hubContext;
  private readonly IObjectMapper objectMapper;

  public SystemHubContext(
      IVportalLogger<SystemHubContext> logger,
      ISystemAppHubContext hubContext,
      IObjectMapper objectMapper)
  {
    this.logger = logger;
    this.hubContext = hubContext;
    this.objectMapper = objectMapper;
  }

  public async Task PushAsync(List<Guid> targetUsers, PushNotificationValueObject notification)
  {
    try
    {
      var mapped = objectMapper.Map<PushNotificationValueObject, PushNotificationDto>(notification);
      await hubContext.SendToAsync("SystemMessage", mapped, targetUsers);
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
