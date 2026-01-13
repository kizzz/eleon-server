using System.Threading.Tasks;

namespace VPortal.TenantManagement.Module.TenantAppearance
{
  public interface ITenantAppearanceAppService
  {
    Task<TenantAppearanceSettingDto> GetTenantAppearanceSettings();
    Task<bool> UpdateTenantAppearanceSettings(UpdateTenantAppearanceSettingRequest request);
  }
}
