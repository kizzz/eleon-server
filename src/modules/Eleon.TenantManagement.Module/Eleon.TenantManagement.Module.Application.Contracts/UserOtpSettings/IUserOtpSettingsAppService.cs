using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.TenantManagement.Module.UserOtpSettings
{
  public interface IUserOtpSettingsAppService : IApplicationService
  {
    Task<UserOtpSettingsDto> GetUserOtpSettings(Guid userId);
    Task<bool> SetUserOtpSettings(UserOtpSettingsDto userOtpSettings);
  }
}
