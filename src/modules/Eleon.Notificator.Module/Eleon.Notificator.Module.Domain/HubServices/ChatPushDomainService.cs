using Logging.Module;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using VPortal.Notificator.Module.Services;

namespace VPortal.Notificator.Module.DomainServices
{
  public class ChatPushDomainService : DomainService
  {
    private readonly IVportalLogger<ChatPushDomainService> _logger;
    private readonly IChatHubContext hubContext;

    public ChatPushDomainService(
        IVportalLogger<ChatPushDomainService> logger,
        IChatHubContext hubContext)
    {
      _logger = logger;
      this.hubContext = hubContext;
    }

    public async Task PushMessage(ChatPushMessageEto message, List<Guid> audienceUserIds, List<string> audienceRoles, List<Guid> audienceOrgUnits)
    {
      try
      {
        await hubContext.PushMessage(message, audienceUserIds, audienceRoles, audienceOrgUnits);
      }
      catch (Exception ex)
      {
        _logger.Capture(ex);
      }

    }
  }
}
