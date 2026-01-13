using System.Diagnostics;
using System.Net.Mime;
using Logging.Module.ErrorHandling.Enrichers;
using Logging.Module.ErrorHandling.Helpers;
using Logging.Module.ErrorHandling.Mappers;
using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Logging.Module.ErrorHandling.Handlers;

/// <summary>
/// Global exception handler that uses ProblemDetails and IProblemDetailsService.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IExceptionMapper _exceptionMapper;
    private readonly IErrorEnricher _errorEnricher;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;
    private readonly ErrorHandlingOptions _options;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        IExceptionMapper exceptionMapper,
        IErrorEnricher errorEnricher,
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment,
        IOptions<ErrorHandlingOptions> options)
    {
        _problemDetailsService = problemDetailsService;
        _exceptionMapper = exceptionMapper;
        _errorEnricher = errorEnricher;
        _logger = logger;
        _environment = environment;
        _options = options.Value;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
            return false;

        try
        {
            // Map exception to HTTP status and app code
            var (httpStatus, appCode, title) = _exceptionMapper.Map(exception);

            // Get trace ID for logging
            var traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

            // Log the exception
            _logger.LogError(exception,
                "Unhandled exception. traceId={TraceId} appCode={AppCode} httpStatus={HttpStatus} path={Path}",
                traceId, appCode, httpStatus, httpContext.Request.Path.Value);

            // Set HTTP status code
            httpContext.Response.StatusCode = httpStatus;

            // Set app code header
            httpContext.Response.Headers["X-Error-Code"] = appCode;

            // Check if HTML is requested (browser request)
            var isHtmlRequested = IsHtmlRequested(httpContext);

            // Create ProblemDetails
            var problemDetails = new ProblemDetails
            {
                Status = httpStatus,
                Title = title,
                Instance = httpContext.Request.Path,
                Detail = _environment.IsDevelopment() || _options.IncludeExceptionDetails
                    ? exception.Message
                    : (_options.IsFriendlyErrors ? GetFriendlyMessage(exception, appCode) : "An unexpected error occurred.")
            };

            // Add app code to extensions
            problemDetails.Extensions["code"] = appCode;

            // Enrich with safe extensions
            _errorEnricher.Enrich(problemDetails, httpContext, exception);

            // Add exception details in development
            if (_environment.IsDevelopment() || _options.IncludeExceptionDetails)
            {
                var (message, stackTrace, data) = SafeExceptionCollector.Collect(exception, _options);
                problemDetails.Extensions["exceptionMessage"] = message;
                problemDetails.Extensions["stackTrace"] = stackTrace;
                
                if (data.Count > 0)
                {
                    problemDetails.Extensions["exceptionData"] = data;
                }
            }

            // Handle HTML response for browser requests
            if (isHtmlRequested && _options.EnableHtmlErrorPages)
            {
                var (message, stackTrace, data) = SafeExceptionCollector.Collect(exception, _options);
                var friendlyMessage = GetFriendlyMessage(exception, appCode);
                
                var html = SafeHtmlErrorPageGenerator.Generate(
                    httpStatus,
                    title,
                    message,
                    friendlyMessage,
                    stackTrace,
                    data,
                    _options,
                    _environment);

                httpContext.Response.ContentType = MediaTypeNames.Text.Html;
                await httpContext.Response.WriteAsync(html, cancellationToken);
                return true;
            }

            // Use ProblemDetailsService for JSON response
            var problemDetailsContext = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            };

            return await _problemDetailsService.TryWriteAsync(problemDetailsContext);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An error occurred while handling an exception.");
            return false;
        }
    }

    private static bool IsHtmlRequested(HttpContext context)
    {
        var acceptHeader = context.Request.Headers.Accept;
        if (acceptHeader.Contains(MediaTypeNames.Text.Html))
            return true;

        foreach (var accept in acceptHeader)
        {
            if (accept?.Contains(MediaTypeNames.Text.Html) == true)
                return true;
        }

        return false;
    }

    private string GetFriendlyMessage(Exception exception, string appCode)
    {
        // Try to get friendly message from exception data
        string? friendlyMsg = null;
        if (exception.Data != null && exception.Data.Contains("FriendlyMessage") && exception.Data["FriendlyMessage"] is string fm)
        {
            friendlyMsg = fm;
        }

        if (!string.IsNullOrEmpty(friendlyMsg))
        {
            return friendlyMsg;
        }

        // Default friendly message with exception ID
        var exceptionId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
        return $"An unexpected error has occurred. Please contact support with error code {appCode} and exception ID {exceptionId}.";
    }
}
