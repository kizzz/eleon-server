using Messaging.Module.Messages;
using Messaging.Module.ETO;
using System.Text.Json;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.MultiTenancy;

namespace Common.EventBus.Module;
public class SystemEventMessageManager : ITransientDependency
{
  public SystemEventMsg GenerateMessage(object eventData)
  {
    var dataType = eventData.GetType();
    if (dataType.IsGenericType)
    {
      var genericTypeDefinition = dataType.GetGenericTypeDefinition();

      if (genericTypeDefinition == typeof(EntityCreatedEto<>) ||
          genericTypeDefinition == typeof(EntityUpdatedEto<>) ||
          genericTypeDefinition == typeof(EntityDeletedEto<>))
      {
        var entityProperty = dataType.GetProperty("Entity");
        if (entityProperty != null)
        {
          var entityValue = entityProperty.GetValue(eventData);

          return new SystemEventMsg(ParseEventName(genericTypeDefinition, entityValue), Serialiaze(entityValue));
        }
        else
        {
          return new SystemEventMsg(ParseEventName(genericTypeDefinition), Serialiaze());
        }
      }

      return new SystemEventMsg(dataType.Name, Serialiaze(eventData));
    }
    return new SystemEventMsg(dataType.Name, Serialiaze(eventData));
  }

  private string ParseEventName(Type genericTypeDefinition, object? data = null)
  {
    string opName;
    if (genericTypeDefinition == typeof(EntityCreatedEto<>)) opName = "Created:";
    else if (genericTypeDefinition == typeof(EntityDeletedEto<>)) opName = "Deleted:";
    else if (genericTypeDefinition == typeof(EntityUpdatedEto<>)) opName = "Updated:";
    else opName = genericTypeDefinition.Name;

    if (data is EntityEto entityEto)
    {
      return opName + entityEto.EntityType?.Split('.').Last() ?? string.Empty;
    }
    return opName + data?.GetType().Name ?? "null";
  }

  private string Serialiaze(object? data = null)
  {
    string serialized;
    if (data == null)
    {
      serialized = string.Empty;
    }
    else if (data is EntityEto entityEto)
    {
      serialized = JsonSerializer.Serialize(new CustomEntityEto
      {
        Name = entityEto.EntityType?.Split(".").Last() ?? string.Empty,
        Type = entityEto.EntityType ?? string.Empty,
        Id = entityEto.KeysAsString
      },
      new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      });
    }
    else
    {
      serialized = JsonSerializer.Serialize(data, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      });
    }

    return serialized;
  }
}
