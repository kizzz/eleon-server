using Eleon.Logging.Lib.SystemLog.Logger;
using Logging.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messager.Module.Telegram;
public static class TelegramHelper
{
  public static async Task<bool> SendAsync(string botToken, string chatId, string message, int deleteAfterSec = 0)
  {
    try
    {
      using var client = new HttpClient();

      var sendUri = $"https://api.telegram.org/bot{botToken}/sendMessage";


      var body = new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", message },
                { "parse_mode", "HTML" }
            };

      var response = await client.PostAsync(sendUri, new FormUrlEncodedContent(body));
      response.EnsureSuccessStatusCode();

      var json = await response.Content.ReadAsStringAsync();
      var sendResp = JsonSerializer.Deserialize<TelegramSendResponse>(json);

      if (sendResp == null || !sendResp.ok)
        throw new Exception($"Telegram send failed: {json}");

      var messageId = sendResp.result.message_id;

      // if DeleteAfterSec is specified → schedule deletion
      if (deleteAfterSec > 0)
      {
        _ = Task.Run(async () =>
        {
          try
          {
            await Task.Delay(TimeSpan.FromSeconds(deleteAfterSec));
            var delUri = $"https://api.telegram.org/bot{botToken}/deleteMessage";
            var delBody = new Dictionary<string, string>
                {
                            { "chat_id", chatId },
                            { "message_id", messageId.ToString() }
                };
            await client.PostAsync(delUri, new FormUrlEncodedContent(delBody));
          }
          catch (Exception ex)
          {
            EleonsoftLog.Error("Failed to delete telegram message", ex);
            throw;
          }
        });
      }

      return sendResp.ok;
    }
    catch (Exception ex)
    {
      EleonsoftLog.Error("Error sending telegram notification", ex);
      throw;
    }
  }
}


public class TelegramSendResponse
{
  public bool ok { get; set; }
  public TelegramMessage result { get; set; }
}

public class TelegramMessage
{
  public int message_id { get; set; }
}
