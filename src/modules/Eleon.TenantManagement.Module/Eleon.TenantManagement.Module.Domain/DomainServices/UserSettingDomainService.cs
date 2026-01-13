using Common.Module.Constants;
using Common.Module.Helpers;
using Common.Module.ValueObjects;
using Logging.Module;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.SettingManagement;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Infrastructure.Module;
using VPortal.Infrastructure.Module.Domain.DomainServices;
using VPortal.Infrastructure.Module.Entities;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Localization;
using VPortal.TenantManagement.Module.Repositories;
using VPortal.TenantManagement.Module.ValueObjects;

namespace VPortal.TenantManagement.Module.DomainServices
{

    
    public class UserSettingDomainService : DomainService
    {
        private readonly IVportalLogger<UserSettingDomainService> logger;
        private readonly ISettingManager settingManager;
        private readonly ICurrentUser currentUser;
        private readonly IOrganizationUnitRepository organizationUnitRepository;
        private readonly OrganizationUnitDomainService organizationUnitDomainService;
        private readonly IObjectMapper mapper;
        private readonly IStringLocalizer<TenantManagementResource> l;
        private readonly IUserSettingRepository userSettingRepository;

        public UserSettingDomainService(
            IVportalLogger<UserSettingDomainService> logger,
            ISettingManager settingManager,
            ICurrentUser currentUser,
            IOrganizationUnitRepository organizationUnitRepository,
            OrganizationUnitDomainService organizationUnitDomainService,
            IdentityUserManager userManager,
            IObjectMapper mapper,
            IStringLocalizer<TenantManagementResource> L,
            IUserSettingRepository userSettingRepository)
        {
            this.logger = logger;
            this.settingManager = settingManager;
            this.currentUser = currentUser;
            this.organizationUnitRepository = organizationUnitRepository;
            this.organizationUnitDomainService = organizationUnitDomainService;
            this.mapper = mapper;
            l = L;
            this.userSettingRepository = userSettingRepository;
        }

        public async Task<ResultValueObject<string>> GetAppearanceSetting(string appId)
        {
            ResultValueObject<string> result = null;
            try
            {
                var resultJson = await settingManager.GetOrNullForUserAsync("Appearance", (Guid)currentUser.Id);
                if (!string.IsNullOrWhiteSpace(resultJson))
                {
                    try
                    {
                        var settingsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(resultJson);
                        if (settingsDict != null && settingsDict.TryGetValue(appId, out var appSetting))
                        {
                            result = ResultHelper.Ok(appSetting);
                        }
                        else
                        {
                            result = ResultHelper.Fail<string>("Setting not found for appId");
                        }
                    }
                    catch (JsonException)
                    {
                        result = ResultHelper.Fail<string>("Invalid JSON format in settings");
                    }
                }
                else
                {
                    result = ResultHelper.Fail<string>("No appearance settings found");
                }
            }
            catch (Exception e)
            {
                return ResultHelper.Fail<string>(e.Message);
            }
            finally
            {
            }
            return result;
        }

        public async Task<ResultValueObject<string>> SetAppearance(string settingValueObject, string appId)
        {
            ResultValueObject<string> result = null;
            try
            {
                var existingSettingsJson = await settingManager.GetOrNullForUserAsync("Appearance", (Guid)currentUser.Id);
                Dictionary<string, string> settingsDict;

                if (!string.IsNullOrWhiteSpace(existingSettingsJson))
                {
                    try
                    {
                        settingsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(existingSettingsJson) ?? new Dictionary<string, string>();
                    }
                    catch (JsonException)
                    {
                        settingsDict = new Dictionary<string, string>(); // Reset if JSON is invalid
                    }
                }
                else
                {
                    settingsDict = new Dictionary<string, string>();
                }

                settingsDict[appId] = settingValueObject;

                var updatedSettingsJson = JsonSerializer.Serialize(settingsDict);
                await settingManager.SetForUserAsync((Guid)currentUser.Id, "Appearance", updatedSettingsJson);

                result = ResultHelper.Ok(settingValueObject);
            }
            catch (Exception e)
            {
                logger.Capture(e);
                result = ResultHelper.Fail<string>(e.Message);
            }
            finally
            {
            }
            return result;
        }


        public async Task<UserSettingEntity> GetUserSettingByUserId(Guid userId)
        {
            UserSettingEntity result = null;
            try
            {
                result = await userSettingRepository.GetUserSettingByUserId(userId);
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }

            return result;
        }

        public async Task<UserSettingEntity> SetUserSetting(UserSettingEntity setting)
        {
            UserSettingEntity result = null;
            try
            {
                if (!currentUser.Id.HasValue)
                {
                    throw new AbpAuthorizationException();
                }
                var userSetting = await userSettingRepository.FindAsync(setting.Id);
                if (userSetting != null)
                {
                    userSetting.TwoFaNotificationType = setting.TwoFaNotificationType;
                    result = await userSettingRepository.UpdateAsync(userSetting, true);
                }
                else
                {
                    var newSetting = new UserSettingEntity(Guid.NewGuid());
                    newSetting.UserId = setting.UserId;
                    newSetting.TwoFaNotificationType = setting.TwoFaNotificationType;
                    result = await userSettingRepository.InsertAsync(newSetting, true);
                }
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }

            return result;
        }

        public async Task<string> GetUserSettingAsync(Guid userId, string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            try
            {
                return await settingManager.GetOrNullForUserAsync(
                            name,
                            userId);
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }

            return null;
        }

        public async Task SetUserSettingAsync(Guid userId, string name, string value)
        {
            ArgumentNullException.ThrowIfNull(name);

            try
            {
                await settingManager.SetForUserAsync(
                            userId,
                            name,
                            value,
                            true);
            }
            catch (Exception e)
            {
                logger.Capture(e);
            }
            finally
            {
            }
        }
    }
}
