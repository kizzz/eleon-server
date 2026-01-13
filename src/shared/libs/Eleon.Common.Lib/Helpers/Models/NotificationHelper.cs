using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messager.Module.Abstractions;
public class SendMessageRequest
{
  public string Message { get; set; }
  public Dictionary<string, object> Parameters { get; set; }
}


public static class SendMessageRequestBuildingExtensions
{
  #region General
  public static SendMessageRequest AsSendMessageRequest(this string message)
  {
    return new SendMessageRequest
    {
      Message = message,
      Parameters = new Dictionary<string, object>()
    };
  }

  private static void EnsureParametersInitialized(SendMessageRequest request)
  {
    if (request.Parameters == null)
      request.Parameters = new Dictionary<string, object>();
  }

  public static SendMessageRequest WithContentType(this SendMessageRequest request, string contentType)
  {
    EnsureParametersInitialized(request);
    request.Parameters["ContentType"] = contentType;
    return request;
  }

  public static string GetParsedContentType(this SendMessageRequest request)
  {
    if (request.Parameters != null && request.Parameters.TryGetValue("ContentType", out var contentTypeObj) && contentTypeObj is string contentType && !string.IsNullOrWhiteSpace(contentType))
    {
      return contentType.ToLower();
    }

    return "text";
  }

  #endregion

  #region Telegram Specific
  public static SendMessageRequest WithChatId(this SendMessageRequest request, string chatId)
  {
    EnsureParametersInitialized(request);

    request.Parameters["ChatId"] = chatId;
    return request;
  }

  public static SendMessageRequest WithDeleteAfterSec(this SendMessageRequest request, int deleteAfterSec)
  {
    EnsureParametersInitialized(request);
    request.Parameters["DeleteAfterSec"] = deleteAfterSec;
    return request;
  }

  public static SendMessageRequest WithBotToken(this SendMessageRequest request, string botToken)
  {
    EnsureParametersInitialized(request);
    request.Parameters["BotToken"] = botToken;
    return request;
  }

  public static string? GetBotToken(this SendMessageRequest request)
  {
    if (request.Parameters != null && request.Parameters.TryGetValue("BotToken", out var botTokenObj) && botTokenObj is string botToken && !string.IsNullOrWhiteSpace(botToken))
    {
      return botToken;
    }
    return null;
  }

  public static string? GetChatId(this SendMessageRequest request)
  {
    if (request.Parameters != null && request.Parameters.TryGetValue("ChatId", out var chatIdObj) && chatIdObj is string chatId && !string.IsNullOrWhiteSpace(chatId))
    {
      return chatId;
    }
    return null;
  }

  public static int GetDeleteAfterSec(this SendMessageRequest request)
  {
    if (request.Parameters != null && request.Parameters.TryGetValue("DeleteAfterSec", out var deleteAfterSecObj) && deleteAfterSecObj is int deleteAfterSec && deleteAfterSec > 0)
    {
      return deleteAfterSec;
    }
    return 0;
  }

  #endregion

  #region Email Specific
  public static SendMessageRequest WithRecepients(this SendMessageRequest request, List<string> recepients)
  {
    EnsureParametersInitialized(request);
    request.Parameters["Recepients"] = recepients;
    return request;
  }

  public static SendMessageRequest WithRecepients(this SendMessageRequest request, string recepient, string serparator = ";")
  {
    EnsureParametersInitialized(request);
    request.Parameters["Recepients"] = recepient.Split(serparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    return request;
  }

  public static List<string> GetRecepients(this SendMessageRequest request)
  {
    if (request.Parameters != null && request.Parameters.TryGetValue("Recepients", out var recepientsObj) && recepientsObj is List<string> recepients && recepients.Any())
    {
      return recepients;
    }
    return new List<string>();
  }
  #endregion
}
