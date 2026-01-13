using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VPortal.Notificator.Module.Chat
{
  public interface IChatHubConnectionStore
  {
    Task AddConnectedUser(Guid userId);
    void RemoveConnectedUser(Guid userId);
    List<Guid> GetConnectedUsers(List<string> roles, List<Guid> exceptUsers, List<Guid> orgUnitIds, bool isPublic);
  }
}
