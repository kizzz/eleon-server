using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.Chat;

namespace VPortal.Notificator.Module.Hubs
{
  public class ChatAppHubContext : IChatAppHubContext, ITransientDependency
  {
    private readonly IVportalLogger<ChatAppHubContext> logger;
    private readonly IHubContext<ChatHub> hub;
    private readonly IChatHubConnectionStore connectionStore;

    public ChatAppHubContext(IVportalLogger<ChatAppHubContext> logger, IHubContext<ChatHub> hub, IChatHubConnectionStore connectionStore)
    {
      this.logger = logger;
      this.hub = hub;
      this.connectionStore = connectionStore;
    }

    public async Task PushMessage(ChatPushMessageEto message, List<Guid> audienceUserIds, List<string> audienceRoles, List<Guid> audienceOrgUnits)
    {
      try
      {
        var connectedUserIds = connectionStore.GetConnectedUsers(audienceRoles, audienceUserIds, audienceOrgUnits, message.ChatRoom.IsPublic);
        var targetIds = audienceUserIds.Concat(connectedUserIds).ToList();
        var muted = new HashSet<Guid>(message.MutedUsers);
        foreach (var userId in targetIds)
        {
          var dto = new ChatPushMessageDto()
          {
            ChatRoom = message.ChatRoom,
            Message = message.Message,
            IsChatMuted = muted.Contains(userId),
          };
          var user = hub.Clients.User(userId.ToString());
          await user.SendAsync("PushMessage", message);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
