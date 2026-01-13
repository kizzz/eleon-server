using Logging.Module;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using VPortal.Notificator.Module.Services;

namespace VPortal.Notificator.Module.Chat
{
  public class ChatHubContext : IChatHubContext, ITransientDependency
  {
    private readonly IChatAppHubContext hub;
    private readonly IVportalLogger<ChatHubContext> logger;
    private readonly IObjectMapper mapper;

    public ChatHubContext(
        IChatAppHubContext hub,
        IVportalLogger<ChatHubContext> logger,
        IObjectMapper mapper)
    {
      this.hub = hub;
      this.logger = logger;
      this.mapper = mapper;
    }

    public async Task PushMessage(ChatPushMessageEto message, List<Guid> audienceUserIds, List<string> audienceRoles, List<Guid> audienceOrgUnits)
    {
      try
      {
        await hub.PushMessage(message, audienceUserIds, audienceRoles, audienceOrgUnits);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
