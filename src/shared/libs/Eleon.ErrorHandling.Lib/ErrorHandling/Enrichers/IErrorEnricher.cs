using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logging.Module.ErrorHandling.Enrichers;

/// <summary>
/// Enriches ProblemDetails with safe contextual information (traceId, instance, tenantId, etc.).
/// </summary>
public interface IErrorEnricher
{
    /// <summary>
    /// Enriches a ProblemDetails instance with safe extensions.
    /// </summary>
    /// <param name="problemDetails">The ProblemDetails to enrich.</param>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="exception">The exception (may be null).</param>
    void Enrich(ProblemDetails problemDetails, HttpContext httpContext, Exception? exception);
}
