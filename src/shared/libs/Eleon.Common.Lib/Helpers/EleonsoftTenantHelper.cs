using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EleonsoftSdk.Helpers;
public static class EleonsoftTenantHelper
{
  public static string ExtractHostnameFromContext(HttpContext httpContext)
  {
    if (httpContext.Request.Headers.TryGetValue("X-Forwarded-Host", out var forwardedHosts))
    {
      return $"https://{forwardedHosts.FirstOrDefault()}";
    }
    if (httpContext.Request.Headers.TryGetValue("host", out var hosts))
    {
      return $"https://{hosts.FirstOrDefault()}";
    }

    if (httpContext.Request.Query.TryGetValue("ReturnUrl", out var redirectUrls))
    {
      var decodedUrl = HttpUtility.UrlDecode(redirectUrls.FirstOrDefault());
      var redirectUrlQuery = HttpUtility.ParseQueryString(decodedUrl);
      var innerRedirectUris = redirectUrlQuery.GetValues("redirect_uri");
      if (innerRedirectUris?.Any() == true)
      {
        return HttpUtility.UrlDecode(innerRedirectUris.First());
      }
    }

    if (httpContext.Request.Query.TryGetValue("redirect_uri", out var redirectUris))
    {
      return redirectUris.FirstOrDefault();
    }

    if (httpContext.Request.Headers.TryGetValue("Referer", out var referers))
    {
      string referer = referers.First();
      if (referer.EndsWith('/'))
      {
        referer = referer[..^1];
      }

      return referer;
    }

    if (httpContext.Request.Headers.TryGetValue("Origin", out var origins))
    {
      return origins.FirstOrDefault();
    }

    return null;
  }
}
