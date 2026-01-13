using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace ServicesOrchestrator.HealthChecks;

public interface IAlertService
{
  Task AlertAsync(Dictionary<string, string> args);
}

[ExposeServices(typeof(IAlertService))]
public class AlertService : IAlertService, ITransientDependency
{
  private readonly IServiceProvider _serviceProvider;
  private readonly IConfiguration _configuration;
  private readonly DebugNotificator _debugNotificator;

  public AlertService(IServiceProvider serviceProvider, IConfiguration configuration, DebugNotificator debugNotificator)
  {
    _serviceProvider = serviceProvider;
    _configuration = configuration;
    _debugNotificator = debugNotificator;
  }

  public async Task AlertAsync(Dictionary<string, string> args)
  {
    await _debugNotificator.DebugAsync(new Messaging.Module.ETO.EleonsoftNotification
    {
      Message = args.GetValueOrDefault("message", "Alert"),
      Priority = NotificationPriority.High,
      Type = new SystemNotificationType
      {
        ExtraProperties = args,
        LogLevel = SystemLogLevel.Critical,
        WriteLog = true
      },
      RunImmidiate = true,
      Id = Guid.NewGuid()
    });
  }
}
