using Common.Module.Constants;
using Eleon.Logging.Lib.SystemLog.Logger;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messager.Module.Abstractions;
using EleonsoftSdk.modules.Messager.Module.Statics;
using EleonsoftSdk.modules.Messager.Module.Telegram;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Logging.Module.SystemLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json;

namespace EleonsoftSdk.modules.Jobs.Module;

public class ServiceSendNotificationJob : DefaultBackgroundJob
{
  internal static string ServiceName = string.Empty;
  private readonly IJsonSerializer _jsonSerializer;

  public ServiceSendNotificationJob(
      ILogger<DefaultBackgroundJob> logger,
      IDistributedEventBus eventBus,
      IJsonSerializer jsonSerializer) : base(logger, eventBus)
  {
    _jsonSerializer = jsonSerializer;
  }

  protected override string Type => $"{ServiceName}SendNotification";

  protected override async Task<JobResult> ProcessExecutionAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(ServiceName))
      {
        EleonsoftLog.Warn("ServiceSendNotification job was not configured but was executed");
        return new JobResult(false, "ServiceSendNotification job was not configured");
      }

      var notification = _jsonSerializer.Deserialize<EleonsoftNotification>(execution.StartExecutionParams);

      await EventBus.PublishAsync(new AddNotificationMsg
      {
        Notification = notification,
      });

      return new JobResult(true, "Notification sent successfully");
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error occurred while processing job.");
      return new JobResult(false, ex.Message);
    }
  }
}

public static class ServiceSendNotificationJobExtensions
{
  public static IServiceCollection AddSendNotificationJob(this IServiceCollection services, string serviceName)
  {
    ServiceSendNotificationJob.ServiceName = serviceName;
    services.AddTransient<ServiceSendNotificationJob>();

    return services;
  }
}
