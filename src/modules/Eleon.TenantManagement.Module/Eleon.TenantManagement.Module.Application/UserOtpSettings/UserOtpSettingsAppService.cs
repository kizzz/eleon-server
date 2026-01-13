using Logging.Module;
using System;
using System.Threading.Tasks;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Entities;

namespace VPortal.TenantManagement.Module.UserOtpSettings
{
  public class UserOtpSettingsAppService : TenantManagementAppService, IUserOtpSettingsAppService
  {
    private readonly IVportalLogger<UserOtpSettingsAppService> logger;
    private readonly UserOtpSettingsDomainService domainService;

    public UserOtpSettingsAppService(
        IVportalLogger<UserOtpSettingsAppService> logger,
        UserOtpSettingsDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<UserOtpSettingsDto> GetUserOtpSettings(Guid userId)
    {
      UserOtpSettingsDto result = null;
      try
      {
        var entity = await domainService.GetUserOtpSettings(userId);
        result = ObjectMapper.Map<UserOtpSettingsEntity, UserOtpSettingsDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> SetUserOtpSettings(UserOtpSettingsDto userOtpSettings)
    {
      bool result = false;
      try
      {
        var entity = ObjectMapper.Map<UserOtpSettingsDto, UserOtpSettingsEntity>(userOtpSettings);
        await domainService.SetUserOtpSettings(entity);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
