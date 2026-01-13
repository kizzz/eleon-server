using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace EleonsoftSdk.Overrides
{
  public class ServiceClaimTransformation : IClaimsTransformation
  {

    public ServiceClaimTransformation()
    {
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
      var identity = principal.Identities.FirstOrDefault();

      if (identity != null)
      {
        var sub = identity.Claims.FirstOrDefault(t => t.Type == "sub");
        if (sub != null)
        {
          identity.AddClaim(new Claim(AbpClaimTypes.UserId, sub.Value));
        }
        //var usernameClaim = identity.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier);
        //if (usernameClaim != null)
        //{
        //    var dbUser = await _userManager.FindByNameAsync(usernameClaim.Value);
        //    if (dbUser != null)
        //    {
        //        identity.RemoveClaim(usernameClaim);
        //        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, dbUser.Id.ToString()));
        //    }
        //}
      }

      return principal;
    }
  }
}
