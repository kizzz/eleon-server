

namespace EventManagementModule.Module.Domain.Shared.Constants;
public static class EventManagementDefaults
{
  public const string SystemQueueName = "SYSTEM_QUEUE";
  public const string SystemQueueDisplayName = "System Queue";
  public const string ErrorQueueName = "ERROR_QUEUE";
  public const string ErrorEventName = "ErrorEvent";

  public static class QueueStatuses
  {
    public const string Ok = "Ok";
    public const string NotFound = "NotFound";
  }

  public const int MinMessagesLimit = 100;
  public const int MinSystemQueueMessagesLimit = 1000;
  public const int MaxMessagesLimit = 1_000_000;
  public const int MaxReceiveMessagesCount = 1000;
  public const int DefaultSystemQueueLimit = 10_000;

  public const string ForwardingSeparator = ";";
  public const string ForwardingAll = "*";

  public static bool IsSystemQueue(string queueName)
  {
    return queueName == SystemQueueName || queueName == ErrorQueueName;
  }
}
