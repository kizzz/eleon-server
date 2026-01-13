namespace Eleoncore.SDK.CoreEvents;

public class ScheduleQueueOptions
{
  public List<ScheduleQueueOptionsEntry> Queues { get; set; } = new List<ScheduleQueueOptionsEntry>();
}

public class ScheduleQueueOptionsEntry
{
  public required string QueueName { get; set; }
  public int MessagesLimit { get; set; } = 100;
  public int ScheduleTime { get; set; } = 60;
  public string Forwarding { get; set; } = "*";
  public int RecieveMessagesCount { get; set; } = 20;
}
