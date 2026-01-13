using Microsoft.AspNetCore.WebUtilities;

namespace Logging.Module.ErrorHandling.Constants;

/// <summary>
/// Maps application error codes (600+) to messages. For HTTP status codes (100-599), use ReasonPhrases.GetReasonPhrase().
/// </summary>
public static class StatusCodeMessageDictionary
{
  // Keep only application-specific status codes (600+)
  private static readonly IReadOnlyDictionary<int, string> AppCodeMessages = new Dictionary<int, string>()
    {
        { EleonsoftStatusCodes.Proxy.ProxyInternalError, "Unexpected proxy error" },
        { EleonsoftStatusCodes.Proxy.ProxyConfigurationError, "Proxy configuration error" },
        { EleonsoftStatusCodes.Proxy.GeneratingProxySourcesError, "Generating proxy sources error" },
        { EleonsoftStatusCodes.Proxy.ProxyForbiddenRequest, "Proxy forbidden response" },
        { EleonsoftStatusCodes.Proxy.ProxyResourceNotFoundError, "Proxy route not found" },
        { EleonsoftStatusCodes.Proxy.SslCertificateValidationFailed, "SSL certificate validation failed" },
        { EleonsoftStatusCodes.Proxy.ForwardingFailed, "Request forwarding failed" },

        { EleonsoftStatusCodes.Default.DefaultServerError, "Internal Server Error" },
        { EleonsoftStatusCodes.Default.TenantWasNotResoleved, "Tenant not resolved" },
        { EleonsoftStatusCodes.Default.SdkError, "Sdk Error" },
        { EleonsoftStatusCodes.Default.BadConfiguration, "Bad Configuration" },

        { EleonsoftStatusCodes.Default.UnhealthyService, "Unhealthy Service" },
    };

  private const string DefaultMessage = "Undefined error";

  /// <summary>
  /// Gets the message for a status code. For HTTP status codes (100-599), uses ReasonPhrases.GetReasonPhrase().
  /// For application codes (600+), uses the internal mapping.
  /// </summary>
  public static string GetStatusCodeMessage(int statusCode)
  {
    // For HTTP status codes, use built-in reason phrases
    if (statusCode >= 100 && statusCode < 600)
    {
      var reasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);
      if (!string.IsNullOrEmpty(reasonPhrase))
      {
        return reasonPhrase;
      }
    }

    // For app codes (600+), use our mapping
    if (AppCodeMessages.TryGetValue(statusCode, out var message))
    {
      return message;
    }

    return DefaultMessage;
  }
}
