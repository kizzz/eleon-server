using Microsoft.AspNetCore.Identity;

namespace VPortal.Identity.Module.CustomCredentials
{
  public class CustomSignInResult
  {
    public SignInResult SignInResult { get; set; }
    public string SuccessMessage { get; set; }
    public string ErrorMessage { get; set; }
    public bool RequireSecureApi { get; set; }
    public bool RequireReload { get; set; }
    public bool RequirePeriodicPasswordChange { get; set; }
    public bool TwoFactorRequired { get; set; }
    public string UserEmail { get; set; }
    public string UserName { get; set; }
    public string UserPhone { get; set; }
    public bool IsUserEmailConfirmed { get; set; }
    public bool IsUserPhoneConfirmed { get; set; }
    public bool IsNewUser { get; set; }
    public string Recipient { get; set; }

    public CustomSignInResult(SignInResult signInResult)
    {
      SignInResult = signInResult;
    }
  }
}
