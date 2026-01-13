using Common.Module.Constants;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messager.Module.Abstractions;
using EleonsoftSdk.modules.Messager.Module.Statics;
using EleonsoftSdk.modules.Messager.Module.Telegram;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Jobs;

public class TestJobParams
{
  public string ChatId { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
  public int DelaySeconds { get; set; } = 0;
  public int FailsCount { get; set; } = 0;
  public int ExecutionNumber { get; set; } = 0; // auto-incremented
  public string Type { get; set; } = "email"; // email, telegram, etc.
  public string Recepients { get; set; } = string.Empty; // comma-separated list of emails/usernames/ids
  public string BotToken { get; set; } = string.Empty;
}

public class TestNotificationJob : DefaultBackgroundJob
{
  internal static string ServiceName = "Eleonsoft";

  public TestNotificationJob(
      ILogger<DefaultBackgroundJob> logger,
      IDistributedEventBus eventBus,
      IServiceProvider serviceProvider) : base(logger, eventBus)
  {
    ServiceProvider = serviceProvider;
  }

  protected IServiceProvider ServiceProvider { get; }

  protected override string Type => $"{ServiceName}TestJob"; // NotificationAction

  protected override async Task<JobResult> ProcessExecutionAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution)
  {
    var messages = new List<BackgroundJobTextInfoEto>();
    var serializerOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      ReadCommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true
    };

    string? originalParams = execution.StartExecutionParams;
    string? updatedParamsJson = null;

    try
    {
      if (string.IsNullOrWhiteSpace(originalParams))
      {
        messages.Add(Info("StartExecutionParams is empty. Expected JSON payload."));
        // Treat as invalid JSON
        return Finish(false, "Invalid JSON: payload is empty.", messages, updatedParamsJson);
      }

      TestJobParams jobParams = null;
      try
      {
        jobParams = JsonSerializer.Deserialize<TestJobParams>(originalParams, serializerOptions)!;
        ValidateParams(jobParams);
      }
      catch (JsonException jex)
      {
        messages.Add(Error($"Invalid JSON in StartExecutionParams: {jex.Message}"));
        return Finish(false, "Invalid JSON in StartExecutionParams.", messages, updatedParamsJson);
      }

      var executionNumber = jobParams.ExecutionNumber;
      jobParams.ExecutionNumber++;
      updatedParamsJson = JsonSerializer.Serialize(jobParams, serializerOptions);

      await SendMessageSafeAsync($"{ServiceName}: START {Type} #{executionNumber}", jobParams, messages);

      // If executionNumber < failsCount => raise error BEFORE delay
      if (executionNumber < jobParams.FailsCount)
      {
        var failMsg = $"Simulated failure: executionNumber({executionNumber}) < failsCount({jobParams.FailsCount}).";
        messages.Add(Error(failMsg));
        // also try to notify via Telegram (best-effort)
        await SendMessageSafeAsync($"{ServiceName}: END {Type} – FAILED: {failMsg}", jobParams, messages);
        // Throw to mark job as failed
        throw new InvalidOperationException(failMsg);
      }

      if (jobParams.DelaySeconds > 0)
      {
        messages.Add(Info($"Delaying for {jobParams.DelaySeconds} second(s)."));
        await Task.Delay(TimeSpan.FromSeconds(jobParams.DelaySeconds));
      }

      await SendMessageSafeAsync($"{ServiceName}: {jobParams.Message}", jobParams, messages);

      return Finish(true, "Job was completed successfully", messages, updatedParamsJson);
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error occurred while processing job");
      messages.Add(Error($"Unhandled exception: {ex.Message}"));
      return Finish(false, ex.Message, messages, updatedParamsJson);
    }
  }

  private async Task SendMessageSafeAsync(string text, TestJobParams jobParams, List<BackgroundJobTextInfoEto> messages)
  {
    try
    {
      SendMessageRequest request = null;
      if (jobParams.Type == "telegram")
      {
        request = text.AsSendMessageRequest()
            .WithBotToken(jobParams.BotToken)
            .WithChatId(jobParams.ChatId);

      }
      else // "email"
      {
        request = text.AsSendMessageRequest()
            .WithRecepients(jobParams.Recepients);
      }

      var resp = await EleonsoftMessagerHelper.SendMessageAsync(ServiceProvider, request, jobParams.Type);

      messages.Add(resp
          ? Info($"Start/End notification sent.")
          : Warn($"Start/End notification failed"));
    }
    catch (Exception e)
    {
      messages.Add(Warn($"Start/End notification threw: {e.Message}"));
    }
  }

  private static void ValidateParams(TestJobParams jobParams)
  {
    ArgumentNullException.ThrowIfNull(jobParams);

    if (jobParams.Type == "telegram")
    {
      if (string.IsNullOrWhiteSpace(jobParams.ChatId))
        throw new ArgumentException("ChatId is required for Telegram messages.");
      if (string.IsNullOrWhiteSpace(jobParams.BotToken))
        throw new ArgumentException("BotToken is required for Telegram messages.");
    }
    else if (jobParams.Type == "email")
    {
      if (string.IsNullOrWhiteSpace(jobParams.Recepients))
        throw new ArgumentException("Recepients is required for Email messages.");
    }
    else
    {
      if (string.IsNullOrWhiteSpace(jobParams.Recepients))
        throw new ArgumentException("Recepients is required for Email messages.");
    }
  }

  private static BackgroundJobTextInfoEto Info(string text) => new()
  {
    TextMessage = text,
    Type = BackgroundJobMessageType.Info
  };

  private static BackgroundJobTextInfoEto Warn(string text) => new()
  {
    TextMessage = text,
    Type = BackgroundJobMessageType.Warn
  };

  private static BackgroundJobTextInfoEto Error(string text) => new()
  {
    TextMessage = text,
    Type = BackgroundJobMessageType.Error
  };

  private JobResult Finish(bool success, string resultMessage, List<BackgroundJobTextInfoEto> messages, string? updatedParamsJson)
  {
    return new JobResult(success, resultMessage, messages, updatedParamsJson);
  }
}

public static class TelegramNotificationJobExtensions
{
  public static IServiceCollection AddTestTelegramNotificationJob(this IServiceCollection services, string serviceName)
  {
    TestNotificationJob.ServiceName = serviceName;
    services.AddTransient<TestNotificationJob>();

    return services;
  }
}
