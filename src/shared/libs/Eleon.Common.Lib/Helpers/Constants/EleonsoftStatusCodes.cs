

namespace Logging.Module.ErrorHandling.Constants;

public static class EleonsoftStatusCodes
{
  public static class Default
  {
    public const int DefaultServerError = 600;
    public const int TenantWasNotResoleved = 601;
    public const int SdkError = 602;
    public const int BadConfiguration = 603;
    public const int UnhealthyService = 700;
  }

  public static class Proxy
  {
    /// <summary>
    /// When proxy has unexpected error
    /// </summary>
    public const int ProxyInternalError = 550; // 500

    /// <summary>
    /// When proxy has recieved bad configuration
    /// </summary>
    public const int ProxyConfigurationError = 551; // 400

    /// <summary>
    /// Generating response (any files/messages which are genereted by proxy) error
    /// </summary>
    public const int GeneratingProxySourcesError = 552; // 500

    /// <summary>
    /// When proxy has forbidden the request
    /// </summary>
    public const int ProxyForbiddenRequest = 553; // 403

    /// <summary>
    /// When resource was not found by proxy
    /// </summary>
    public const int ProxyResourceNotFoundError = 554; // 404

    /// <summary>
    /// When certificate was invalid
    /// </summary>
    public const int SslCertificateValidationFailed = 555;


    /// <summary>
    /// When any forwarding error occured
    /// </summary>
    public const int ForwardingFailed = 556;
  }
}

