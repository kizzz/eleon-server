namespace Logging.Module.ErrorHandling.Options;

public class ErrorHandlingOptions
{
  // Legacy options (maintained for backward compatibility)
  public bool IsFriendlyErrors { get; set; } = true;
  public int DefaultErrorStatusCode { get; set; } = 500;
  public string DefaultErrorPageTitle { get; set; } = "Internal Server Error";

  // New security and behavior options
  /// <summary>
  /// Include exception details in responses. Should only be true in Development.
  /// </summary>
  public bool IncludeExceptionDetails { get; set; } = false;

  /// <summary>
  /// Include request headers in error responses. Default: false for security.
  /// </summary>
  public bool IncludeRequestHeaders { get; set; } = false;

  /// <summary>
  /// List of header names to redact when including headers. Default includes sensitive headers.
  /// </summary>
  public HashSet<string> RedactedHeaders { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
  {
    "Authorization",
    "Cookie",
    "Set-Cookie",
    "X-Api-Key",
    "X-Auth-Token",
    "X-Forwarded-For",
    "X-Real-IP"
  };

  /// <summary>
  /// Maximum length for string fields in error responses. Prevents unbounded data.
  /// </summary>
  public int MaxFieldLength { get; set; } = 1000;

  /// <summary>
  /// Maximum number of items in collections (arrays, dictionaries) included in error responses.
  /// </summary>
  public int MaxCollectionItems { get; set; } = 50;

  /// <summary>
  /// Maximum depth for inner exception chains. Prevents unbounded recursion.
  /// </summary>
  public int MaxInnerExceptionDepth { get; set; } = 10;

  /// <summary>
  /// Default HTTP status code for unhandled exceptions.
  /// </summary>
  public int DefaultHttpStatus { get; set; } = 500;

  /// <summary>
  /// Default application error code (e.g., "ELEON-0000").
  /// </summary>
  public string DefaultAppCode { get; set; } = "ELEON-0000";

  /// <summary>
  /// Enable HTML error pages. In production, should only be enabled for browser requests.
  /// </summary>
  public bool EnableHtmlErrorPages { get; set; } = true;
}
