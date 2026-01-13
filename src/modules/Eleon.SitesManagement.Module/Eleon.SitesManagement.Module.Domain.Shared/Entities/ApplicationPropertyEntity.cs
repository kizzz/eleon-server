using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.SitesManagement.Module.Entities
{
  public class ApplicationPropertyEntity : FullAuditedEntity<Guid>
  {
    public ApplicationPropertyEntity(Guid id) : base(id) { }
    public ApplicationPropertyEntity() { }
    public Guid? TenantId { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    public string Level { get; set; }
  }
}


