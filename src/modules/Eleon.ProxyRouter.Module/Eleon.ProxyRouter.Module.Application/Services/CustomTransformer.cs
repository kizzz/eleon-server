using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProxyRouter.Minimal.HttpApi.ErrorHandling.Exceptions;
using Yarp.ReverseProxy.Forwarder;

namespace VPortal.Masav.Feature.Module.Transformers
{
  internal class CustomTransformer : HttpTransformer
  {
    public CustomTransformer(string url)
    {
      Url = url;
    }

    public string Url { get; }

    public override async ValueTask TransformRequestAsync(HttpContext httpContext,
        HttpRequestMessage proxyRequest, string destinationPrefix, CancellationToken cancellationToken)
    {
      try
      {
        // Copy all request headers
        await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, cancellationToken);
        proxyRequest.Headers.Host = null;
        try
        {
          proxyRequest.RequestUri = new Uri(Url);
        }
        catch (Exception ex)
        {
          var logger = httpContext.RequestServices.GetRequiredService<ILogger<CustomTransformer>>();
          logger.LogError(ex, "An error has occured when parsing uri '{uri}'.", Url);
          throw new ProxyException($"Invalid request uri: '{Url}' {ex.Message}", ex)
          {
            RequestedResourceUrl = Url
          }
              .WithStatusCode(EleonsoftStatusCodes.Proxy.ProxyConfigurationError)
              .WithFriendlyMessage("Invalid request uri. Check application and resources configuration uris. ");
        }

        // Add X-Forwarded-For header to pass the original client IP address
        var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(remoteIpAddress))
        {
          proxyRequest.Headers.TryAddWithoutValidation("X-Forwarded-For", remoteIpAddress);
        }

        // Add X-Forwarded-Proto header to pass the original protocol (http/https)
        var forwardedProto = httpContext.Request.IsHttps ? "https" : "http";
        proxyRequest.Headers.TryAddWithoutValidation("X-Forwarded-Proto", forwardedProto);

        // Add X-Forwarded-Host header to pass the original host
        var originalHost = httpContext.Request.Host.Value;
        proxyRequest.Headers.TryAddWithoutValidation("X-Forwarded-Host", originalHost);

        proxyRequest.Headers.Host = httpContext.Request.Headers["Host"].ToString();
      }
      catch (ProxyException)
      {
        throw;
      }
      catch (Exception ex)
      {
        var logger = httpContext.RequestServices.GetRequiredService<ILogger<CustomTransformer>>();
        logger.LogError(ex, "An unexpected error has occured while transforming the request");
        throw new ProxyException("An unexpected error has occured while transforming the request. " + ex.Message, ex)
        {
          RequestedResourceUrl = Url
        }
            .WithStatusCode(EleonsoftStatusCodes.Proxy.ProxyInternalError);
      }
    }
  }
}
