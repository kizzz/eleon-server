using System;

namespace VPortal.Infrastructure.Module.AuditLogs
{

  public class AuditLogHeaderDto
  {
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string CorrelationId { get; set; }
    public string ApplicationName { get; set; }
    public string Url { get; set; }
    public string UserName { get; set; }
    public string ClientIpAddress { get; set; }
    public DateTime ExecutionTime { get; set; }
    public int ExecutionDuration { get; set; }
    public string HttpMethod { get; set; }
    public int? HttpStatusCode { get; set; }
  }
}
