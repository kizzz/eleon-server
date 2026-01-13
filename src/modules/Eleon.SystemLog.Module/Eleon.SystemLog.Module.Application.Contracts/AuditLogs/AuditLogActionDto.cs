using System;

namespace VPortal.Infrastructure.Module.AuditLogs
{
  public class AuditLogActionDto
  {
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public Guid AuditLogId { get; set; }
    public string ServiceName { get; set; }
    public string MethodName { get; set; }
    public string Parameters { get; set; }
    public DateTime ExecutionTime { get; set; }
    public int ExecutionDuration { get; set; }
  }
}
