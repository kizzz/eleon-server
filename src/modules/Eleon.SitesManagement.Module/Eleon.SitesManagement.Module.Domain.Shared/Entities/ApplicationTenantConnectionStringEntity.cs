using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.SitesManagement.Module.Entities
{
  public class ApplicationTenantConnectionStringEntity : FullAuditedEntity<Guid>, IMultiTenant
  {
    public string ApplicationName { get; set; }
    public string ConnectionString { get; set; }
    public string Status { get; set; }

    public Guid? TenantId { get; set; }

    public ApplicationTenantConnectionStringEntity(Guid id) : base(id)
    {

    }
  }
}


