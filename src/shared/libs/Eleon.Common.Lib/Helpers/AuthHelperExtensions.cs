using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Auth;
public static class AuthHelperExtensions
{
  public const string ApiKeyIdClaimType = "client_key_id";
  public const string ApiKeyNameClaimType = "client_key_name";
  public const string ApiKeyRefIdClaimType = "client_key_sub";
  public const string ApiKeyTypeClaimType = "client_key_type";

  public static string? GetApiKeyId(this ClaimsPrincipal user)
  {
    return user.FindFirstValue(ApiKeyIdClaimType);
  }

  public static string? GetApiKeyName(this ClaimsPrincipal user)
  {
    return user.FindFirstValue(ApiKeyNameClaimType);
  }

  public static string? GetApiKeyType(this ClaimsPrincipal user)
  {
    return user.FindFirstValue(ApiKeyTypeClaimType);
  }

  public static string? GetApiKeyRefId(this ClaimsPrincipal user)
  {
    return user.FindFirstValue(ApiKeyRefIdClaimType);
  }

  public static string? GetName(this ClaimsPrincipal user)
  {
    if (user.Identity?.IsAuthenticated != true)
      return null;

    if (!string.IsNullOrEmpty(user.Identity.Name))
    {
      return user.Identity.Name;
    }

    return user.GetApiKeyName();
  }

  public static string? GetUserId(this ClaimsPrincipal user)
  {
    if (user.Identity?.IsAuthenticated != true)
      return null;

    var subClaim = user.FindFirst("sub");
    if (subClaim != null)
    {
      return subClaim.Value;
    }

    var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (idClaim != null)
    {
      return idClaim.Value;
    }

    return null;
  }
}
