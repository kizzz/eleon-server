using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace VPortal.Overrides.Account
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  [ExposeServices(typeof(IProfileAppService))]
  [Authorize("NotDriver")]
  public class VportalProfileAppService : ProfileAppService
  {
    public VportalProfileAppService(
        IdentityUserManager userManager,
        //IdentitySecurityLogManager identitySecurityLogManager,
        //IdentityProTwoFactorManager identityProTwoFactorManager,
        IOptions<IdentityOptions> identityOptions)
        : base(userManager,
              //identitySecurityLogManager, identityProTwoFactorManager,
              identityOptions)
    {
    }

    public override Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
    {
      return base.UpdateAsync(input);
    }

    public override Task ChangePasswordAsync(ChangePasswordInput input)
    {
      return base.ChangePasswordAsync(input);
    }

    //public override Task SetTwoFactorEnabledAsync(bool enabled)
    //{
    //    return base.SetTwoFactorEnabledAsync(enabled);
    //}

    //public override Task<bool> GetTwoFactorEnabledAsync()
    //{
    //    return base.GetTwoFactorEnabledAsync();
    //}

    public override Task<ProfileDto> GetAsync()
    {
      return base.GetAsync();
    }
  }
}
