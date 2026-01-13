using System.Collections.Generic;

namespace VPortal.Identity.Module.IdentityServerExtensions.Claims
{
  public class VPortalIdentityClaimsOptions
  {
    /// <summary>
    /// Gets the list of optional claim types that will be added to a token if present in ClaimsIdentity after validation.
    /// </summary>
    public List<string> AdditionalOptionalClaimTypes { get; } = new List<string>();
  }
}
