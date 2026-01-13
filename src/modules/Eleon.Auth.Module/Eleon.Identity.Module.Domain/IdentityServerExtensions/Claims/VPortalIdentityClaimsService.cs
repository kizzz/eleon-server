using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Volo.Abp.IdentityServer;

namespace VPortal.Identity.Module.IdentityServerExtensions.Claims
{
  public class VPortalIdentityClaimsService : AbpClaimsService
  {
    private readonly IOptions<VPortalIdentityClaimsOptions> claimsOptions;

    public VPortalIdentityClaimsService(
        IProfileService profile,
        ILogger<DefaultClaimsService> logger,
        IOptions<AbpClaimsServiceOptions> options,
        IOptions<VPortalIdentityClaimsOptions> claimsOptions)
        : base(profile, logger, options)
    {
      this.claimsOptions = claimsOptions;
    }

    protected override IEnumerable<Claim> GetAdditionalOptionalClaims(ClaimsPrincipal subject)
    {
      var res = base.GetAdditionalOptionalClaims(subject).ToList();
      foreach (var claimName in claimsOptions.Value.AdditionalOptionalClaimTypes)
      {
        var claim = subject.FindFirst(claimName);
        if (claim != null)
        {
          res.Add(claim);
        }
      }

      return res;
    }
  }
}
