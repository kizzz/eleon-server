using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Module.Constants
{
  public class IdentitySettingsConsts
  {
    public const string TwoFactorAuthenticationEnable = "TwoFactorAuthenticationEnable";
    public const string TwoFactorAuthenticationOption = "TwoFactorAuthenticationOption";
    public const string TwoFactoAuthenticationEmailTemplate = "2FA Email";

    public static class TwoFactorAuthenticationOptions
    {
      public const string Mixed = "mixed";
      public const string Email = "email";
      public const string Sms = "sms";
    }

    public const string SmsProviderOption = "SmsProviderOption";
    public const string PasswordEnable = "PasswordEnable";
    public const string SelfRegistrationEnable = "SelfRegistrationEnable";
  }
}
