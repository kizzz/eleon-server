using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Logging.Module.ErrorHandling.Mappers;

/// <summary>
/// Default implementation of IExceptionMapper that maps common exception types to HTTP status codes and app codes.
/// </summary>
public class DefaultExceptionMapper : IExceptionMapper
{
    private readonly ErrorHandlingOptions _options;

    public DefaultExceptionMapper(IOptions<ErrorHandlingOptions> options)
    {
        _options = options.Value;
    }

    public (int HttpStatus, string AppCode, string Title) Map(Exception exception)
    {
        if (exception == null)
        {
            return (_options.DefaultHttpStatus, _options.DefaultAppCode, "Internal Server Error");
        }

        // Map common exception types (check more specific types first)
        return exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "ELEON-AUTH-403", "Forbidden"),
            ArgumentNullException => (StatusCodes.Status400BadRequest, "ELEON-REQ-400", "Bad Request"),
            ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, "ELEON-REQ-400", "Bad Request"),
            ArgumentException => (StatusCodes.Status400BadRequest, "ELEON-REQ-400", "Bad Request"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "ELEON-REQ-400", "Bad Request"),
            NotImplementedException => (StatusCodes.Status501NotImplemented, "ELEON-SRV-501", "Not Implemented"),
            TimeoutException => (StatusCodes.Status504GatewayTimeout, "ELEON-SRV-504", "Gateway Timeout"),
            _ => MapEleonsoftStatusCode(exception)
        };
    }

    private (int HttpStatus, string AppCode, string Title) MapEleonsoftStatusCode(Exception exception)
    {
        // Check if exception has a StatusCode in its Data dictionary
        int statusCode = 0;
        if (exception.Data != null && exception.Data.Contains("StatusCode") && exception.Data["StatusCode"] is int sc)
        {
            statusCode = sc;
        }

        if (statusCode > 0)
        {
            // Map Eleonsoft status codes (600+) to HTTP equivalents
            return statusCode switch
            {
                EleonsoftStatusCodes.Proxy.ProxyInternalError => (StatusCodes.Status500InternalServerError, "ELEON-PROXY-550", "Proxy Internal Error"),
                EleonsoftStatusCodes.Proxy.ProxyConfigurationError => (StatusCodes.Status400BadRequest, "ELEON-PROXY-551", "Proxy Configuration Error"),
                EleonsoftStatusCodes.Proxy.GeneratingProxySourcesError => (StatusCodes.Status500InternalServerError, "ELEON-PROXY-552", "Generating Proxy Sources Error"),
                EleonsoftStatusCodes.Proxy.ProxyForbiddenRequest => (StatusCodes.Status403Forbidden, "ELEON-PROXY-553", "Proxy Forbidden Request"),
                EleonsoftStatusCodes.Proxy.ProxyResourceNotFoundError => (StatusCodes.Status404NotFound, "ELEON-PROXY-554", "Proxy Resource Not Found"),
                EleonsoftStatusCodes.Proxy.SslCertificateValidationFailed => (StatusCodes.Status502BadGateway, "ELEON-PROXY-555", "SSL Certificate Validation Failed"),
                EleonsoftStatusCodes.Proxy.ForwardingFailed => (StatusCodes.Status502BadGateway, "ELEON-PROXY-556", "Forwarding Failed"),
                EleonsoftStatusCodes.Default.DefaultServerError => (StatusCodes.Status500InternalServerError, "ELEON-SRV-600", "Internal Server Error"),
                EleonsoftStatusCodes.Default.TenantWasNotResoleved => (StatusCodes.Status400BadRequest, "ELEON-SRV-601", "Tenant Not Resolved"),
                EleonsoftStatusCodes.Default.SdkError => (StatusCodes.Status500InternalServerError, "ELEON-SRV-602", "SDK Error"),
                EleonsoftStatusCodes.Default.BadConfiguration => (StatusCodes.Status500InternalServerError, "ELEON-SRV-603", "Bad Configuration"),
                EleonsoftStatusCodes.Default.UnhealthyService => (StatusCodes.Status503ServiceUnavailable, "ELEON-SRV-700", "Unhealthy Service"),
                _ => (_options.DefaultHttpStatus, $"ELEON-{statusCode}", "Error")
            };
        }

        // Default fallback
        return (_options.DefaultHttpStatus, _options.DefaultAppCode, "Internal Server Error");
    }
}
