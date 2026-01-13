using EleonsoftProxy.Model;
using System.Text.Json;

namespace Eleoncore.SDK.CoreEvents;
public abstract class MessageHandler<T> : IMessageHandler
{
  public abstract Task HandleAsync(T message);

  public Task HandleAsync(EventManagementModuleFullEventDto message)
  {
    try
    {
      if (message.Name == typeof(T).Name)
      {
        T data = default;
        try
        {
          data = JsonSerializer.Deserialize<T>(message.Message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        catch (Exception)
        {
          throw;
        }

        return HandleAsync(data);
      }
    }
    catch (Exception)
    {
      throw;
    }

    return Task.CompletedTask;
  }
}
