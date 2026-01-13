using Common.Module.Constants;
using EleonsoftModuleCollector.Commons.Module.Constants.IdentitySettings;
using System.Collections.Generic;
using Volo.Abp.Account.Settings;
using Volo.Abp.Identity.Settings;

namespace VPortal.TenantManagement.Module.Settings
{

  public static class IdentitySettingConsts
  {
    public static readonly Dictionary<string, List<string>> SettingGroups = new()
    {
      ["TenantManagement::PermissionGroupCategory:General"] =
        [
            IdentitySettingsConsts.SelfRegistrationEnable,
            ],
      ["TenantManagement::PermissionGroupCategory:User"] =
        [
            IdentitySettingNames.User.IsUserNameUpdateEnabled,
                IdentitySettingNames.User.IsEmailUpdateEnabled,
            ],
      ["TenantManagement::PermissionGroupCategory:SignIn"] =
        [
            //IdentitySettingNames.SignIn.RequireConfirmedEmail,
            //IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber,
            IdentitySettingsConsts.TwoFactorAuthenticationEnable,
                IdentitySettingsConsts.PasswordEnable,
            ],
      ["TenantManagement::PermissionGroupCategory:TwoFactorAuthenticationSettings"] =
        [
            IdentitySettingsConsts.TwoFactorAuthenticationOption,
                IdentitySettingsConsts.SmsProviderOption,
              IdentitySettingsConsts.TwoFactoAuthenticationEmailTemplate,
            ],
      ["TenantManagement::PermissionGroupCategory:Password"] =
        [
            IdentitySettingNames.Password.RequiredLength,
                IdentitySettingNames.Password.RequiredUniqueChars,
                IdentitySettingNames.Password.RequireNonAlphanumeric,
                IdentitySettingNames.Password.RequireLowercase,
                IdentitySettingNames.Password.RequireUppercase,
                IdentitySettingNames.Password.RequireDigit,
            ],
      ["TenantManagement::PermissionGroupCategory:PasswordRenew"] =
        [
            IdentitySettingNames.Password.ForceUsersToPeriodicallyChangePassword,
                IdentitySettingNames.Password.PasswordChangePeriodDays,
            ],
      ["TenantManagement::PermissionGroupCategory:Lockout"] =
        [
            IdentitySettingNames.Lockout.AllowedForNewUsers,
                IdentitySettingNames.Lockout.LockoutDuration,
                IdentitySettingNames.Lockout.MaxFailedAccessAttempts,
            ],
    };

    public static readonly Dictionary<string, IdentitySettingType> SettingTypes = new()
    {
      [IdentitySettingNames.Password.RequiredLength] = IdentitySettingType.Number,
      [IdentitySettingNames.Password.RequiredUniqueChars] = IdentitySettingType.Number,
      [IdentitySettingNames.Password.RequireNonAlphanumeric] = IdentitySettingType.Boolean,
      [IdentitySettingNames.Password.RequireLowercase] = IdentitySettingType.Boolean,
      [IdentitySettingNames.Password.RequireUppercase] = IdentitySettingType.Boolean,
      [IdentitySettingNames.Password.RequireDigit] = IdentitySettingType.Boolean,
      [IdentitySettingNames.Password.ForceUsersToPeriodicallyChangePassword] = IdentitySettingType.Boolean,
      [IdentitySettingNames.Password.PasswordChangePeriodDays] = IdentitySettingType.Number,
      [IdentitySettingNames.Lockout.AllowedForNewUsers] = IdentitySettingType.Boolean,
      [IdentitySettingNames.Lockout.LockoutDuration] = IdentitySettingType.Number,
      [IdentitySettingNames.Lockout.MaxFailedAccessAttempts] = IdentitySettingType.Number,
      //[IdentitySettingNames.SignIn.RequireConfirmedEmail] = IdentitySettingType.Boolean,
      [IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation] = IdentitySettingType.Boolean,
      //[IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber] = IdentitySettingType.Boolean,
      [IdentitySettingNames.User.IsUserNameUpdateEnabled] = IdentitySettingType.Boolean,
      [IdentitySettingNames.User.IsEmailUpdateEnabled] = IdentitySettingType.Boolean,
      [IdentitySettingsConsts.SelfRegistrationEnable] = IdentitySettingType.Boolean,
      [IdentitySettingsConsts.TwoFactorAuthenticationEnable] = IdentitySettingType.Boolean,
      [IdentitySettingsConsts.TwoFactorAuthenticationOption] = IdentitySettingType.String,
      [IdentitySettingsConsts.TwoFactoAuthenticationEmailTemplate] = IdentitySettingType.Template,
      [IdentitySettingsConsts.SmsProviderOption] = IdentitySettingType.String,
      [IdentitySettingsConsts.PasswordEnable] = IdentitySettingType.Boolean,
    };

    public static readonly Dictionary<string, string> SettingNamesOverrides = new()
    {
      [IdentitySettingNames.User.IsEmailUpdateEnabled] = "IdentitySettingNames.User.IsEmailUpdateEnabled",
    };

    public static readonly Dictionary<string, string> SettingDescriptionOverrides = new()
    {
      [IdentitySettingNames.User.IsUserNameUpdateEnabled] = "AllowUsersToChangTheirUsernames.Description",
    };
  }
}
