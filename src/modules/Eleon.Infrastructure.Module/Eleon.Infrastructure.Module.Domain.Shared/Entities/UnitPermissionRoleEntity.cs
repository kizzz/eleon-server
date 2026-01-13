using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Infrastructure.Module.Entities
{
  public class UnitPermissionRoleEntity : FullAuditedEntity<Guid>, IMultiTenant
  {
    public Guid UnitPermissionId { get; set; }
    public Guid RoleId { get; set; }

    public Guid? TenantId { get; set; }
  }
}
