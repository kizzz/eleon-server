using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.TenantManagement.Module.Settings
{
  public interface IIdentitySettingAppService : IApplicationService
  {
    Task<List<IdentitySettingDto>> GetIdentitySettings();
    Task<bool> SetIdentitySettings(SetIdentitySettingsRequest request);
  }
}
