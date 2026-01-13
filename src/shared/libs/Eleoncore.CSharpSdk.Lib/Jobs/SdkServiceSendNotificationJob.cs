using Eleon.Logging.Lib.SystemLog.Logger;
using EleonsoftApiSdk.Helpers;
using EleonsoftProxy.Api;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Messaging.Module.ETO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Json;

namespace EleoncoreAspNetCoreSdk.Jobs;
public class SdkServiceSendNotificationJob : SdkDefaultBackgroundJob
{
  internal static string ServiceName = string.Empty;
  private readonly INotificationsApi _notificationsApi;
  private readonly IJsonSerializer _jsonSerializer;

  public SdkServiceSendNotificationJob(
      ILogger<SdkDefaultBackgroundJob> logger,
      IBackgroundJobApi jobApi,
      INotificationsApi notificationsApi,
      IJsonSerializer jsonSerializer) : base(logger, jobApi)
  {
    _notificationsApi = notificationsApi;
    _jsonSerializer = jsonSerializer;
  }

  protected override string Type => $"{ServiceName}SendNotification";

  protected override async Task<SdkJobResult> HandleJobAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(ServiceName))
      {
        EleonsoftLog.Warn("SdkServiceSendNotification job was not configured but was executed");
        return new SdkJobResult(false, "SdkServiceSendNotification job was not configured");
      }

      var notification = _jsonSerializer.Deserialize<EleonsoftNotification>(execution.StartExecutionParams);

      var notificationDto = notification.ToDto();

      var response = await _notificationsApi.NotificatorNotificationsSendAsync(notificationDto);

      return new SdkJobResult(response.IsSuccessStatusCode, response.IsSuccessStatusCode ? "Notification sent successfully" : "Failed to send notification");
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error occurred while sending notification");
      return new SdkJobResult(false, ex.Message);
    }
  }
}

public static class ServiceSendNotificationJobExtensions
{
  public static IServiceCollection AddSdkSendNotificationJob(this IServiceCollection services, string serviceName)
  {
    SdkServiceSendNotificationJob.ServiceName = serviceName;
    services.AddTransient<SdkServiceSendNotificationJob>();

    return services;
  }
}
