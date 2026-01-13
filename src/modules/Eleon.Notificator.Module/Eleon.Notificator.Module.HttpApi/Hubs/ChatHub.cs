using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using VPortal.Notificator.Module.Chat;

namespace VPortal.Notificator.Module.Hubs
{
  [Authorize]
  [HubRoute("hubs/chat/chat-hub")]
  public class ChatHub : AbpHub
  {
    private readonly IChatHubConnectionStore connectionStore;

    public ChatHub(IChatHubConnectionStore connectionStore)
    {
      this.connectionStore = connectionStore;
    }

    public override async Task OnConnectedAsync()
    {
      await connectionStore.AddConnectedUser(CurrentUser.Id.Value);
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
      connectionStore.RemoveConnectedUser(CurrentUser.Id.Value);
      return base.OnDisconnectedAsync(exception);
    }
  }
}
