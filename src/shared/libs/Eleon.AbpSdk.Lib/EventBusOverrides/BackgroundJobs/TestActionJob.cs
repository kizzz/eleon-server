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
using Volo.Abp.Json;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Jobs;

public class TestActionJobParams
{
  public bool ShouldFail { get; set; } = false;
  public string Message { get; set; } = string.Empty;
  public int DelayInMiliseconds { get; set; } = 0;
}

public class TestActionJob : DefaultBackgroundJob, ITransientDependency
{
  private readonly IJsonSerializer jsonSerializer;
  public TestActionJob(
      ILogger<TestActionJob> logger,
      IDistributedEventBus eventBus,
      IJsonSerializer jsonSerializer) : base(logger, eventBus)
  {
    this.jsonSerializer = jsonSerializer;
  }

  protected override string Type => "TestActionJob"; 

  protected override async Task<JobResult> ProcessExecutionAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution)
  {
    var success = true;
    var log = new List<BackgroundJobTextInfoEto>();

    var message = "";
    try
    {
      var jobParams = jsonSerializer.Deserialize<TestActionJobParams>(execution.StartExecutionParams);

      if (jobParams.DelayInMiliseconds > 0)
      {
        log.Add(new BackgroundJobTextInfoEto
        {
          CreationTime = DateTime.UtcNow,
          TextMessage = $"TestActionJob delayed for {jobParams.DelayInMiliseconds} miliseconds.",
          Type = BackgroundJobMessageType.Info
        });
        await Task.Delay(jobParams.DelayInMiliseconds);
        log.Add(new BackgroundJobTextInfoEto
        {
          CreationTime = DateTime.UtcNow,
          TextMessage = $"TestActionJob delay ended.",
          Type = BackgroundJobMessageType.Info
        });

      }
      if (jobParams.ShouldFail)
      {
        throw new Exception(jobParams.Message);
      }

      message = jobParams.Message;
    }
    catch (Exception ex)
    {
      log.Add(new BackgroundJobTextInfoEto
      {
        TextMessage = ex.Message,
        Type = BackgroundJobMessageType.Error,
      });
      success = false;
    }
    log.Add(new BackgroundJobTextInfoEto
    {
      CreationTime = DateTime.UtcNow,
      TextMessage = $"TestActionJob executed successfully.",
      Type = BackgroundJobMessageType.Info
    });

    return new JobResult(
        Success: success,
        Result: $"{message}",
        Log: log,
        NextExecutionParams: null,
        NextExecutionExtraParams: null,
        CompletedBy: Type);
  }
}
