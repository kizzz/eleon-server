namespace ExternalLogin.Module
{
  public class ExternalLoginSecurityLogActions
  {
    public const string StartRemoteLogout = "StartRemoteLogout";
    public const string RedirectToExternalProviderForLogout = "RedirectToExternalProviderForLogout";
    public const string RedirectToExternalProviderForLogin = "RedirectToExternalProviderForLogin";
    public const string ProvisionExternalUser = "ProvisionExternalUser";
    public const string LoginWithExternalProvider = "LoginWithExternalProvider";
    public const string LogoutFromExternalProvider = "LogoutFromExternalProvider";
    public const string ExternalLoginFailed = "ExternalLoginFailed";
    public const string RequestedTwoFactor = "RequestedTwoFactor";
    public const string TwoFactorFailedAttempt = "TwoFactorFailedAttempt";
    public const string RequestedAccessTokenByApiKey = "RequestedAccessTokenByApiKey";
  }
}
