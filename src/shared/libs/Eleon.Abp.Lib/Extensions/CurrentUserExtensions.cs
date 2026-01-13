using EleonsoftAbp.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace Eleon.AbpSdk.Lib.modules.HostExtensions.Module.Auth;
public static class CurrentUserExtensions
{
  public static string? GetApiKeyId(this ICurrentUser user)
  {
    return user.FindClaimValue(AuthHelperExtensions.ApiKeyIdClaimType);
  }

  public static string? GetApiKeyName(this ICurrentUser user)
  {
    return user.FindClaimValue(AuthHelperExtensions.ApiKeyNameClaimType);
  }

  public static string? GetApiKeyType(this ICurrentUser user)
  {
    return user.FindClaimValue(AuthHelperExtensions.ApiKeyTypeClaimType);
  }

  public static string? GetApiKeyRefId(this ICurrentUser user)
  {
    return user.FindClaimValue(AuthHelperExtensions.ApiKeyRefIdClaimType);
  }

  public static string GetName(this ICurrentUser user)
  {
    if (!user.IsAuthenticated)
      return string.Empty;
    if (!string.IsNullOrEmpty(user.Name))
    {
      return user.Name;
    }
    return user.GetApiKeyName() ?? string.Empty;
  }
}
