using Logging.Module;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

namespace VPortal.Infrastructure.Module.Domain.DomainServices
{
  public static class IdentityUserManagerExtensions
  {
    public static async Task<ClaimsPrincipal> CreatePrincipal(this IdentityUserManager userManager, Guid userId)
    {
      var user = await userManager.GetByIdAsync(userId);
      var roles = await userManager.GetRolesAsync(user);
      var userClaims = user.Claims.Select(x => x.ToClaim());
      var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
      var idClaims = new Claim[]
      {
                new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
                new Claim(AbpClaimTypes.UserName, user.UserName),
      };

      var claims = userClaims.Concat(roleClaims).Concat(idClaims);
      return new ClaimsPrincipal(new[]
      {
                new ClaimsIdentity(claims),
            });
    }
  }
}
