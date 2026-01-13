using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eleon.McpGateway.Module.Infrastructure.Middleware;

public sealed class McpOriginValidationMiddleware
{
    private readonly RequestDelegate next;
    private readonly McpStreamableOptions options;
    private readonly ILogger<McpOriginValidationMiddleware> logger;

    public McpOriginValidationMiddleware(
        RequestDelegate next,
        IOptions<McpStreamableOptions> options,
        ILogger<McpOriginValidationMiddleware> logger)
    {
        this.next = next;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only validate /mcp endpoints
        if (!context.Request.Path.StartsWithSegments("/mcp", StringComparison.OrdinalIgnoreCase))
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        // If no allowed origins configured, allow all (backward compatibility)
        if (options.AllowedOrigins.Count == 0)
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        var origin = context.Request.Headers["Origin"].FirstOrDefault();
        
        // If no origin header, allow (some clients don't send it)
        if (string.IsNullOrWhiteSpace(origin))
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        // Check if origin is allowed
        if (IsOriginAllowed(origin))
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        var correlationId = context.TraceIdentifier;
        logger.LogWarning(
            "Rejected request from unauthorized origin: {Origin} (CorrelationId: {CorrelationId}, Path: {Path})",
            origin, correlationId, context.Request.Path);
        context.Response.StatusCode = 403;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("""{"error":"Origin not allowed"}""", context.RequestAborted).ConfigureAwait(false);
    }

    private bool IsOriginAllowed(string origin)
    {
        foreach (var allowedOrigin in options.AllowedOrigins)
        {
            if (string.IsNullOrWhiteSpace(allowedOrigin))
            {
                continue;
            }

            // Exact match
            if (string.Equals(origin, allowedOrigin, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Wildcard support: https://*.example.com
            if (allowedOrigin.Contains('*'))
            {
                var pattern = allowedOrigin.Replace("*", ".*");
                try
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(origin, $"^{pattern}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Invalid wildcard pattern in allowed origin: {Pattern}", allowedOrigin);
                }
            }
        }

        return false;
    }
}

