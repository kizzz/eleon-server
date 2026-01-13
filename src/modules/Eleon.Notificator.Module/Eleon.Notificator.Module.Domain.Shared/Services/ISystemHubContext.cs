using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Notificator.Module.Eleon.Notificator.Module.Domain.Shared.Services;
public interface ISystemHubContext
{
  Task PushAsync(List<Guid> targetUsers, PushNotificationValueObject notification);
}
