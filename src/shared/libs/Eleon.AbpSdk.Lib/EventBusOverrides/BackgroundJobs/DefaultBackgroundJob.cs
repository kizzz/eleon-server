using Common.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.BackgroundJobs;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftSdk.modules.Helpers.Module;

public record JobResult(
    bool Success,
    string Result,
    List<BackgroundJobTextInfoEto>? Log = null,
    string? NextExecutionParams = null,
    string? NextExecutionExtraParams = null,
    string? CompletedBy = null);

public abstract class DefaultBackgroundJob : IDistributedEventHandler<StartingJobExecutionMsg>
{
  public DefaultBackgroundJob(ILogger<DefaultBackgroundJob> logger, IDistributedEventBus eventBus)
  {
    Logger = logger;
    EventBus = eventBus;
  }

  protected ILogger<DefaultBackgroundJob> Logger { get; }
  protected IDistributedEventBus EventBus { get; }

  protected abstract string Type { get; }
  protected abstract Task<JobResult> ProcessExecutionAsync(BackgroundJobEto job, BackgroundJobExecutionEto execution);

  public async Task HandleEventAsync(StartingJobExecutionMsg eventData)
  {
    var job = eventData.BackgroundJob;

    if (job.Type != Type)
    {
      return;
    }

    Logger.LogInformation("Starting job execution. JobId: {JobId}, ExecutionId: {ExecutionId}", job.Id, eventData.Execution.Id);

    var execution = eventData.Execution;

    var messages = new List<BackgroundJobTextInfoEto>();
    var status = BackgroundJobExecutionStatus.Errored;
    var result = string.Empty;
    var nextExecutionParams = execution.StartExecutionParams;
    var nextExecutionExtraParams = execution.StartExecutionExtraParams;
    var completedBy = Type;

    try
    {
      await EventBus.PublishAsync(new MarkJobExecutionStartedMsg
      {
        JobId = job.Id,
        ExecutionId = execution.Id,
        TenantId = eventData.TenantId,
        TenantName = eventData.TenantName,
      });

      var jobResult = await ProcessExecutionAsync(eventData.BackgroundJob, eventData.Execution);
      status = jobResult.Success ? BackgroundJobExecutionStatus.Completed : BackgroundJobExecutionStatus.Errored;
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
      Logger.LogError(ex, "Error occurred while processing job.");
      status = BackgroundJobExecutionStatus.Errored;
      messages.Add(new BackgroundJobTextInfoEto()
      {
        Type = BackgroundJobMessageType.Error,
        TextMessage = ex.Message,
        CreationTime = DateTime.UtcNow,
      });
    }
    finally
    {
      var message = new BackgroundJobExecutionCompletedMsg
      {
        Type = job.Type,
        BackgroundJobId = job.Id,
        ExecutionId = execution.Id,
        ParamsForRetryExecution = nextExecutionParams,
        ExtraParamsForRetryExecution = nextExecutionExtraParams,
        Status = status,
        Messages = messages,
        TenantId = eventData.TenantId,
        TenantName = eventData.TenantName,
        Result = result,
        IsManually = true,
        CompletedBy = completedBy
      };

      var serializedMessage = string.Empty;

      try
      {
        serializedMessage = JsonSerializer.Serialize(message);
      }
      catch (Exception ex)
      {
        serializedMessage = $"{{ \"Error\": \"Failed to serialize message: {ex.Message}\" }}";
      }

      Logger.LogInformation("Publishing job execution completed event. JobId: {JobId}, ExecutionId: {ExecutionId}. Message: {Message}", job.Id, eventData.Execution.Id, serializedMessage);

      await EventBus.PublishAsync(message);

      Logger.LogDebug("Completed job execution. JobId: {JobId}, ExecutionId: {ExecutionId}", job.Id, eventData.Execution.Id);
    }
  }
}
