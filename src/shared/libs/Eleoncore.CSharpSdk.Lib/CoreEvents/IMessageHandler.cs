using EleonsoftProxy.Model;

namespace Eleoncore.SDK.CoreEvents;

public interface IMessageHandler
{
  Task HandleAsync(EventManagementModuleFullEventDto message);
}
