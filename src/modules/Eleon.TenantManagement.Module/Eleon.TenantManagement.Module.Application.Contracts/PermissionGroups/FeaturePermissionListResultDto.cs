using Volo.Abp.PermissionManagement;

namespace VPortal.TenantManagement.Module.PermissionGroups
{
  public class FeaturePermissionListResultDto : GetPermissionListResultDto
  {
    public ProviderInfoDto AllGrantedByProvider { get; set; }
  }
}
