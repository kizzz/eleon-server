using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Eleoncore.SDK.CoreEvents;

[DisallowConcurrentExecution]
internal class QueueScheduleJob : IJob
{
  private readonly IServiceScopeFactory _serviceScopeFactory;
  private readonly ILogger<QueueScheduleJob> _logger;

  public QueueScheduleJob(IServiceScopeFactory serviceScopeFactory, ILogger<QueueScheduleJob> logger)
  {
    _serviceScopeFactory = serviceScopeFactory;
    _logger = logger;
  }

  public async Task Execute(IJobExecutionContext context)
  {
    try
    {
      var jobDataMap = context.JobDetail.JobDataMap;
      var queueName = jobDataMap.GetString(nameof(ScheduleQueueOptionsEntry.QueueName));
      var receiveMessagesCount = jobDataMap.GetInt(nameof(ScheduleQueueOptionsEntry.RecieveMessagesCount));
      var messagesLimit = jobDataMap.GetInt(nameof(ScheduleQueueOptionsEntry.MessagesLimit));
      var forwarding = jobDataMap.GetString(nameof(ScheduleQueueOptionsEntry.Forwarding));

      if (string.IsNullOrEmpty(queueName))
      {
        _logger.LogError("QueueName is required for QueueScheduleJob");
        return;
      }

      using var scope = _serviceScopeFactory.CreateScope();
      var scheduler = scope.ServiceProvider.GetRequiredService<QueueScheduler>();

      await scheduler.ScheduleAsync(queueName, receiveMessagesCount, messagesLimit, forwarding ?? "*");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while executing QueueScheduleJob");
      throw;
    }
  }
}
