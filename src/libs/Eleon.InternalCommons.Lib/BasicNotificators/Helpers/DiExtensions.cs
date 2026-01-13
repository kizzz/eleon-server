using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
using VPortal.Notificator.Module.Notificators.Implementations;

namespace EleonsoftModuleCollector.Commons.Module.BasicNotificators.Helpers;
public static class DiExtensions
{
  public static void AddBasicNotificatorServices(this IServiceCollection services)
  {
    services.AddTransient<NotificatorBaseHelperService>();
    services.AddTransient<DebugNotificator>();
    services.AddTransient<TelegramNotificator>();
    services.AddTransient<SmtpNotificator>();
    services.AddTransient<AzureNotificator>();
  }
}
