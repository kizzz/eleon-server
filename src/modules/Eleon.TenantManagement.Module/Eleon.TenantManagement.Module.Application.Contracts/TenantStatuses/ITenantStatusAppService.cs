using System;
using System.Threading.Tasks;

namespace VPortal.TenantManagement.Module.TenantStatuses
{
  public interface ITenantStatusAppService
  {
    Task<bool> SuspendTenant(Guid tenantId);
    Task<bool> UnsuspendTenant(Guid tenantId);
  }
}
