using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftSdk.modules.Helpers.Module;

public record SdkJobResult(
    bool Success,
    string Result,
    List<BackgroundJobsBackgroundJobMessageDto>? Log = null,
    string? NextExecutionParams = null,
    string? NextExecutionExtraParams = null,
    string? CompletedBy = null);

public abstract class SdkDefaultBackgroundJob : IDistributedEventHandler<StartingJobExecutionMsg>
{
  public SdkDefaultBackgroundJob(ILogger<SdkDefaultBackgroundJob> logger, IBackgroundJobApi jobApi)
  {
    Logger = logger;
    JobApi = jobApi;
  }

  protected ILogger<SdkDefaultBackgroundJob> Logger { get; }
  public IBackgroundJobApi JobApi { get; }

  protected abstract string Type { get; }
  protected abstract Task<SdkJobResult> HandleJobAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution);


  public async Task HandleEventAsync(StartingJobExecutionMsg eventData)
  {
    var job = eventData.BackgroundJob;

    if (job.Type != Type)
    {
      return;
    }

    JobApi.UseApiAuth();

    var execution = eventData.Execution;

    var messages = new List<BackgroundJobsBackgroundJobMessageDto>();
    var status = EleoncoreBackgroundJobExecutionStatus.Errored;
    var result = string.Empty;
    var nextExecutionParams = execution.StartExecutionParams;
    var nextExecutionExtraParams = execution.StartExecutionExtraParams;
    var completedBy = Type;

    try
    {
      await JobApi.BackgroundJobsBackgroundJobMarkExecutionStartedAsync(job.Id, execution.Id);

      var jobResult = await HandleJobAsync(eventData.BackgroundJob, eventData.Execution);
      status = jobResult.Success ? EleoncoreBackgroundJobExecutionStatus.Completed : EleoncoreBackgroundJobExecutionStatus.Errored;
      messages.AddRange(jobResult.Log ?? []);
      result = jobResult.Result;

      if (jobResult.NextExecutionParams != null)
      {
        nextExecutionParams = jobResult.NextExecutionParams;
      }

      if (jobResult.NextExecutionExtraParams != null)
      {
        nextExecutionExtraParams = jobResult.NextExecutionExtraParams;
      }

      if (jobResult.CompletedBy != null)
      {
        completedBy = jobResult.CompletedBy;
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Unhandled exception occurred");
      status = EleoncoreBackgroundJobExecutionStatus.Errored;
      messages.Add(new BackgroundJobsBackgroundJobMessageDto()
      {
        MessageType = EleoncoreBackgroundJobMessageType.Error,
        TextMessage = ex.Message,
        CreationTime = DateTime.UtcNow,
      });
    }
    finally
    {
      var message = new BackgroundJobsBackgroundJobExecutionCompleteDto
      {
        Type = job.Type,
        BackgroundJobId = job.Id,
        ExecutionId = execution.Id,
        ParamsForRetryExecution = nextExecutionParams,
        ExtraParamsForRetryExecution = nextExecutionExtraParams,
        Status = status,
        Messages = messages,
        Result = result,
      };

      await JobApi.BackgroundJobsBackgroundJobCompleteAsync(message);
    }
  }
}
