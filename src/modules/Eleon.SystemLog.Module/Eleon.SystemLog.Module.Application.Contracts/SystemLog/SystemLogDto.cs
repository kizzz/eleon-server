using Common.Module.Constants;
using Eleon.Logging.Lib.SystemLog.Contracts;
using System;

namespace VPortal.DocMessageLog.Module.DocMessageLogs;

public class SystemLogDto
{
  public Guid Id { get; set; }
  public string Message { get; set; }
  public SystemLogLevel LogLevel { get; set; }
  public string ApplicationName { get; set; }
  public string InitiatorId { get; set; }
  public string InitiatorType { get; set; } // User, System, etc.
  public DateTime CreationTime { get; set; }
  public bool IsArchived { get; set; }
  public int Count { get; set; }
  public DateTime? LastModificationTime { get; set; }
  public Guid? LastModifierId { get; set; }
}
