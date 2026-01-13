using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.FileManager.Module.Entities
{
  public class PhysicalFolderEntity : FullAuditedAggregateRoot<string>
  {
    public string Name { get; set; }
    public long SystemFolderName { get; set; }
    public string ParentId { get; set; }
    [NotMapped]
    public PhysicalFolderEntity Parent { get; set; }
    public string Size { get; set; }
    [NotMapped]
    public List<PhysicalFolderEntity> Children { get; set; }
  }
}
