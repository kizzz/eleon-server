using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftAbp.EleonsoftIdentity.Sessions;
using EleonsoftSdk.modules.Helpers.Module;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;

[JsonPolymorphic(TypeDiscriminatorPropertyName = nameof(AbstractNotificationType.Type))]
[JsonDerivedType(typeof(EmailNotificationType), EmailNotificationType.TypeConst)]
[JsonDerivedType(typeof(MessageNotificationType), MessageNotificationType.TypeConst)]
[JsonDerivedType(typeof(SystemNotificationType), SystemNotificationType.TypeConst)]
[JsonDerivedType(typeof(PushNotificationType), PushNotificationType.TypeConst)]
[JsonDerivedType(typeof(TwoFactorNotificationType), TwoFactorNotificationType.TypeConst)]
[JsonDerivedType(typeof(SmsNotificationType), SmsNotificationType.TypeConst)]
[JsonDerivedType(typeof(SocialNotificationType), SocialNotificationType.TypeConst)]
public abstract class AbstractNotificationType
{
  public abstract string Type { get; }

  [JsonExtensionData]
  public Dictionary<string, JsonElement>? ExtensionData { get; init; }

  public override string ToString()
  {
    return $"Notification Type: {Type}";
  }
}



public class EmailNotificationType : AbstractNotificationType
{
  public const string TypeConst = "Email";

  public override string Type => TypeConst;

  public bool IsHtml { get; set; }

  // Key: FileName, Value: Base64 string
  public Dictionary<string, string> Attachments { get; set; }

  public string Subject { get; set; }
}

public class MessageNotificationType : AbstractNotificationType
{
  public const string TypeConst = "Message";
  public override string Type => TypeConst;
  public string? ApplicationName { get; set; } = StaticServicesAccessor.GetConfiguration()?.GetValue<string>("NotificationApplication");
  public bool IsLocalizedData { get; set; } = false;
  public bool IsRedirectEnabled { get; set; }
  public string TemplateName { get; set; }
  public string RedirectUrl { get; set; }
  public List<string> DataParams { get; set; }
}

public class SystemNotificationType : AbstractNotificationType
{
  public const string TypeConst = "System";
  public override string Type => TypeConst;
  public Dictionary<string, string> ExtraProperties { get; set; } = new Dictionary<string, string>();
  public SystemLogLevel LogLevel { get; set; } = SystemLogLevel.Info;
  public bool WriteLog { get; set; } = true;
}

public class PushNotificationType : AbstractNotificationType
{
  public string? ApplicationName { get; set; } = StaticServicesAccessor.GetConfiguration()?.GetValue<string>("NotificationApplication");
  public bool IsLocalizedData { get; set; } = false;
  public List<string> DataParams { get; set; }

  public const string TypeConst = "Push";
  public override string Type => TypeConst;
}

public class TwoFactorNotificationType : AbstractNotificationType
{
  public const string TypeConst = "TwoFactor";
  public override string Type => TypeConst;
  public string UserName { get; set; }
  public FullSessionInformation Session { get; set; }
}

public class SmsNotificationType : AbstractNotificationType
{
  public const string TypeConst = "SMS";
  public override string Type => TypeConst;
}

public class SocialNotificationType : AbstractNotificationType
{
  public const string TypeConst = "Social";
  public override string Type => TypeConst;
  public string Platform { get; set; } // e.g., Telegram, WhatsUp, Facebook, Twitter
  public string ChannelId { get; set; }
}

public static class NotificationTypeHelper
{
  private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
  public static string ToJsonString(this AbstractNotificationType type)
  {
    return JsonSerializer.Serialize(type, type.GetType(), _jsonSerializerOptions);
  }

  public static AbstractNotificationType FromJsonString(string json)
  {
    // get type and base on type deserialize to specific class

    using (JsonDocument doc = JsonDocument.Parse(json))
    {
      if (doc.RootElement.TryGetProperty("Type", out JsonElement typeElement))
      {
        string type = typeElement.GetString();
        return type switch
        {
          EmailNotificationType.TypeConst => JsonSerializer.Deserialize<EmailNotificationType>(json, _jsonSerializerOptions)!,
          MessageNotificationType.TypeConst => JsonSerializer.Deserialize<MessageNotificationType>(json, _jsonSerializerOptions)!,
          SystemNotificationType.TypeConst => JsonSerializer.Deserialize<SystemNotificationType>(json, _jsonSerializerOptions)!,
          PushNotificationType.TypeConst => JsonSerializer.Deserialize<PushNotificationType>(json, _jsonSerializerOptions)!,
          TwoFactorNotificationType.TypeConst => JsonSerializer.Deserialize<TwoFactorNotificationType>(json, _jsonSerializerOptions)!,
          SmsNotificationType.TypeConst => JsonSerializer.Deserialize<SmsNotificationType>(json, _jsonSerializerOptions)!,
          SocialNotificationType.TypeConst => JsonSerializer.Deserialize<SocialNotificationType>(json, _jsonSerializerOptions)!,
          _ => throw new NotSupportedException($"Notification type '{type}' is not supported."),
        };
      }
      else
      {
        throw new JsonException("Missing 'Type' property in JSON.");
      }
    }
  }
}
