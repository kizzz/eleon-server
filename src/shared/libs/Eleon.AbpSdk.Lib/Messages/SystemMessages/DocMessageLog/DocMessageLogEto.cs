using Common.Module.Constants;

namespace Messaging.Module.ETO;

public class DocMessageLogEto
{
  public Guid? Id { get; set; }
  public DateTime CreationTime { get; set; }
  public required string Message { get; set; }
  public DocMessageLogType MessageType { get; set; }
  public required string DocumentId { get; set; }
  public required string DocType { get; set; }
  public DocMessageLogStatus Status { get; set; }
  public virtual required string JobType { get; set; }
  public virtual required string RefKey { get; set; }
  public virtual Guid? JobId { get; set; }
}
