using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Settings;

namespace VPortal.Identity.Module.SignIn
{
  public class SignInManager : AbpSignInManager
  {
    public SignInManager(
        IdentityUserManager userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<Volo.Abp.Identity.IdentityUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<Volo.Abp.Identity.IdentityUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<Volo.Abp.Identity.IdentityUser> confirmation,
        IOptions<AbpIdentityOptions> options,
        ISettingProvider settingProvider)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation, options, settingProvider)
    {
    }

    protected override async Task<SignInResult> PreSignInCheck(Volo.Abp.Identity.IdentityUser user)
    {
      // From Microsoft implementation (grandparent SignInManager)
      if (!await CanSignInAsync(user))
      {
        return SignInResult.NotAllowed;
      }

      if (await IsLockedOut(user))
      {
        return await LockedOut(user);
      }

      // From Abp implementation (parent SignInManager), except periodic change
      if (!user.IsActive)
      {
        // Note: logger is passed to base class, not available here directly
        return SignInResult.NotAllowed;
      }

      return null;
    }
  }
}
