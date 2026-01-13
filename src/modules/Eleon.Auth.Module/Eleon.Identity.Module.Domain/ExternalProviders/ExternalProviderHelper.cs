using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using Volo.Abp.Security.Claims;

namespace VPortal.Identity.Module.ExternalProviders
{
  internal class ExternalProviderHelper
  {
    public static Volo.Abp.Identity.IdentityUser CreateUserFromExternalData(Guid userId, Guid? tenantId, ExternalLoginInfo externalLoginInfo)
    {
      // TODO: switch by auth scheme
      return CreateUserFromAzureData(userId, tenantId, externalLoginInfo);
    }

    private static Volo.Abp.Identity.IdentityUser CreateUserFromAzureData(Guid userId, Guid? tenantId, ExternalLoginInfo externalLoginInfo)
    {
      string userName = new(externalLoginInfo.Principal.FindFirstValue(AbpClaimTypes.UserName).Where(char.IsLetterOrDigit).ToArray());
      string name = externalLoginInfo.Principal.FindFirstValue(AbpClaimTypes.Name) ?? userName;
      string email = externalLoginInfo.Principal.Identity.Name;
      return new Volo.Abp.Identity.IdentityUser(userId, userName, email, tenantId)
      {
        Name = name,
      };
    }
  }
}
