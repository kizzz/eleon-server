using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VPortal.Notificator.Module.Services
{
  public interface IChatHubContext
  {
    Task PushMessage(ChatPushMessageEto message, List<Guid> audienceUserIds, List<string> audienceRoles, List<Guid> audienceOrgUnits);
  }
}
