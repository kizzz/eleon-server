using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.Identity.Module.IdentityServerExtensions.Token
{
  public class VPortalTokenRequestValidator : ICustomTokenRequestValidator
  {
    public VPortalTokenRequestValidator()
    {

    }

    public async Task ValidateAsync(CustomTokenRequestValidationContext context)
    {
      var result = context.Result.ValidatedRequest.ClientClaims;
    }
  }
}
