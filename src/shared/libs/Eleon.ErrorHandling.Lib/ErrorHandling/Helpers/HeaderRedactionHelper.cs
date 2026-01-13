using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Http;

namespace Logging.Module.ErrorHandling.Helpers;

/// <summary>
/// Helper for safely redacting sensitive headers from error responses.
/// </summary>
public static class HeaderRedactionHelper
{
    /// <summary>
    /// Gets sanitized headers based on options. Only includes safe headers or redacts sensitive ones.
    /// </summary>
    public static Dictionary<string, string> GetSanitizedHeaders(IHeaderDictionary headers, ErrorHandlingOptions options)
    {
        if (headers == null || !options.IncludeRequestHeaders)
            return new Dictionary<string, string>();

        var sanitized = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var header in headers)
        {
            // Skip redacted headers
            if (options.RedactedHeaders.Contains(header.Key))
            {
                sanitized[header.Key] = "[REDACTED]";
                continue;
            }

            // Join multiple values safely
            var value = string.Join(", ", header.Value.ToArray());
            
            // Truncate if too long
            if (value.Length > options.MaxFieldLength)
            {
                value = value.Substring(0, options.MaxFieldLength) + "... [TRUNCATED]";
            }

            sanitized[header.Key] = value;
        }

        return sanitized;
    }
}
