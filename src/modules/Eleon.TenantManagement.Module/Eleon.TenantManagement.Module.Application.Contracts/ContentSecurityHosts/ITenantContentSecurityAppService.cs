using System.Threading.Tasks;

namespace VPortal.TenantManagement.Module.ContentSecurityHosts
{
  public interface ITenantContentSecurityAppService
  {
    Task<bool> AddTenantContentSecurityHost(AddTenantContentSecurityHostDto input);
    Task<bool> RemoveTenantContentSecurityHost(RemoveTenantContentSecurityHostDto input);
    Task<bool> UpdateTenantContentSecurityHost(UpdateTenantContentSecurityHostDto input);
  }
}
