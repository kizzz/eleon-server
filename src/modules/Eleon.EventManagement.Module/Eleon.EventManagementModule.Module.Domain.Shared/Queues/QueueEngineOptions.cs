namespace EventManagementModule.Module.Domain.Shared.Queues;

public sealed class QueueEngineOptions
{
  public QueueEngineMode QueueEngineMode { get; set; } = QueueEngineMode.DualWrite;
  public ConsumerMode ConsumerMode { get; set; } = ConsumerMode.SqlClaim;
  public bool EnableCompression { get; set; } = false;
  public bool ShadowVerificationEnabled { get; set; } = false;
  public int ShadowVerificationSampleRate { get; set; } = 100;
}

public enum QueueEngineMode : byte
{
  LinkedList = 0,
  SqlClaim = 1,
  DualWrite = 2
}

public enum ConsumerMode : byte
{
  LinkedList = 0,
  SqlClaim = 1
}
