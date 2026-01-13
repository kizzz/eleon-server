using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.TenantManagement.Module.PermissionGroups
{
  public interface IFeaturePermissionListAppService : IApplicationService
  {
    Task<FeaturePermissionListResultDto> GetAsync(string providerName, string providerKey);
  }
}
