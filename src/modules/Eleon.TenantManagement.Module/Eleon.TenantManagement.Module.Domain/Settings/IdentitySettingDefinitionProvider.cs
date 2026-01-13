using Common.Module.Constants;
using Volo.Abp.Localization;
using Volo.Abp.Settings;
using VPortal.TenantManagement.Module.Localization;
// using VPortal.Identity.Module.Localization;

namespace VPortal.Identity.Module.Settings;

public class IdentitySettingDefinitionProvider : SettingDefinitionProvider
{
  public override void Define(ISettingDefinitionContext context)
  {
    /* Define module settings here.
     * Use names from IdentitySettings class.
     */
    context.Add(new SettingDefinition(
        IdentitySettingsConsts.TwoFactorAuthenticationEnable,
        "False",
        L("TwoFactorAuthenticationEnable"),
        L("TwoFactorAuthentication:Description"),
        true));

    context.Add(new SettingDefinition(
        IdentitySettingsConsts.TwoFactorAuthenticationOption,
        IdentitySettingsConsts.TwoFactorAuthenticationOptions.Mixed,
        L("TwoFactorAuthenticationOption"),
        L("TwoFactorAuthenticationOption:Description"),
        true));

    context.Add(new SettingDefinition(
        IdentitySettingsConsts.SmsProviderOption,
        "DEFAULT",
        L("SmsProviderOption"),
        L("SmsProviderOption:Description"),
        true));
    context.Add(new SettingDefinition(
      IdentitySettingsConsts.TwoFactoAuthenticationEmailTemplate,
      "Default",
      L("EmailTemplate"),
      L("EmailTemplate:Description"),
      true
      ));

    context.Add(new SettingDefinition(
        IdentitySettingsConsts.PasswordEnable,
        "True",
        L("PasswordEnable"),
        L("PasswordEnable:Description"),
        true));

    context.Add(new SettingDefinition(
        IdentitySettingsConsts.SelfRegistrationEnable,
        "False",
        L("SelfRegistrationEnable"),
        L("SelfRegistrationEnable:Description"),
        true));
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<TenantManagementResource>(name); // IdentityResource
  }
}
