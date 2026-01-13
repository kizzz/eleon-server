using System.Diagnostics;
using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Logging.Module.ErrorHandling.Enrichers;

/// <summary>
/// Default implementation of IErrorEnricher that adds safe contextual information.
/// </summary>
public class DefaultErrorEnricher : IErrorEnricher
{
    private readonly ErrorHandlingOptions _options;
    private readonly IHostEnvironment _environment;

    public DefaultErrorEnricher(IOptions<ErrorHandlingOptions> options, IHostEnvironment environment)
    {
        _options = options.Value;
        _environment = environment;
    }

    public void Enrich(ProblemDetails problemDetails, HttpContext httpContext, Exception? exception)
    {
        if (problemDetails == null || httpContext == null)
            return;

        // Add trace ID (from Activity.Current or TraceIdentifier)
        var traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;
        problemDetails.Extensions["traceId"] = traceId;

        // Add instance (request path)
        problemDetails.Instance = httpContext.Request.Path;

        // Add tenant ID if available (common in multi-tenant apps)
        var tenantId = GetTenantId(httpContext);
        if (tenantId != null)
        {
            problemDetails.Extensions["tenantId"] = tenantId;
        }

        // Add correlation ID if available
        var correlationId = GetCorrelationId(httpContext);
        if (correlationId != null)
        {
            problemDetails.Extensions["correlationId"] = correlationId;
        }

        // Add exception type in development only
        if (_environment.IsDevelopment() && exception != null)
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().FullName;
        }
    }

    private string? GetTenantId(HttpContext httpContext)
    {
        // Try common tenant ID locations
        if (httpContext.Items.TryGetValue("__TenantId", out var tenantId))
            return tenantId?.ToString();

        // Try header
        if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValue))
            return headerValue.ToString();

        return null;
    }

    private string? GetCorrelationId(HttpContext httpContext)
    {
        // Try common correlation ID locations
        if (httpContext.Items.TryGetValue("__CorrelationId", out var correlationId))
            return correlationId?.ToString();

        // Try header
        if (httpContext.Request.Headers.TryGetValue("X-Correlation-Id", out var headerValue))
            return headerValue.ToString();

        return null;
    }
}
