using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Account.Emailing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace VPortal.Overrides.Account
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  [ExposeServices(typeof(IAccountAppService))]
  [Authorize("NotDriver")]
  public class VportalAccountAppService : AccountAppService
  {
    public VportalAccountAppService(
        IdentityUserManager userManager,
        IAccountEmailer accountEmailer,
        IIdentityRoleRepository roleRepository,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<IdentityOptions> identityOptions,
        IIdentitySecurityLogRepository securityLogRepository)
        : base(userManager,
              //phoneService,
              roleRepository,
              accountEmailer,
              identitySecurityLogManager,
              //accountProfilePictureContainer, settingManager,
              identityOptions
              //, securityLogRepository
              )
    {
    }

    //public override Task ConfirmEmailAsync(ConfirmEmailInput input)
    //{
    //    return base.ConfirmEmailAsync(input);
    //}

    //public override Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input)
    //{
    //    return base.ConfirmPhoneNumberAsync(input);
    //}

    public override Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
      return base.RegisterAsync(input);
    }

    public override Task ResetPasswordAsync(ResetPasswordDto input)
    {
      return base.ResetPasswordAsync(input);
    }

    //public override Task SetProfilePictureAsync(ProfilePictureInput input)
    //{
    //    return base.SetProfilePictureAsync(input);
    //}
  }
}
