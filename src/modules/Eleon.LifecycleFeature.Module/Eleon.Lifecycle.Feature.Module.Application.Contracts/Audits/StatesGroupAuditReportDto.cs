using Common.Module.Constants;
using Volo.Abp.Application.Dtos;

namespace VPortal.Lifecycle.Feature.Module.Audits
{
  public class StatesGroupAuditReportDto : EntityDto<Guid>
  {
    public string DocumentId { get; set; }
    public string DocumentObjectType { get; set; }
    public string GroupName { get; set; }
    public LifecycleStatus Status { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime StatusDate { get; set; }
    public string Role { get; set; }
    public Dictionary<string, object> ExtraProperties { get; set; }
  }
}
