using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Users;
using VPortal.Infrastructure.Module.Result;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.TenantManagement.Module.UserSettings
{
  public interface IUserSettingsAppService : IApplicationService
  {
    Task<ResultDto<string>> GetAppearanceSetting(string appId);
    Task<ResultDto<string>> SetAppearanceSetting(string appearanceSettingsDto, string appId);
    Task<UserSettingDto> GetUserSettingByUserId(Guid userId);
    Task<UserSettingDto> SetUserSettings(UserSettingDto userSettingDto);
    Task<string> GetCurrentUserSettingAsync(string name);
    Task SetCurrentUserSettingAsync(string name, string value);
    Task<string> GetUserSettingAsync(Guid userId, string name);
    Task SetUserSettingAsync(Guid userId, string name, string value);
  }
}
