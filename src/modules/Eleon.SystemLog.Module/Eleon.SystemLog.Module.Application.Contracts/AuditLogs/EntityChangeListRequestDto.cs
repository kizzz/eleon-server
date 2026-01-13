using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace VPortal.Infrastructure.Module.AuditLogs
{
  public class EntityChangeListRequestDto : PagedAndSortedResultRequestDto
  {
    public new string Sorting { get; set; }
    public new int MaxResultCount { get; set; }
    public new int SkipCount { get; set; }
    public Guid? AuditLogId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public EntityChangeType? EntityChangeType { get; set; }
    public string EntityId { get; set; }
    public string EntityTypeFullName { get; set; }
  }
}
