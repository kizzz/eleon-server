using System.Net;
using System.Text;
using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Options;
using Microsoft.Extensions.Hosting;

namespace Logging.Module.ErrorHandling.Helpers;

/// <summary>
/// Generates safe HTML error pages with proper encoding and no secrets.
/// </summary>
public static class SafeHtmlErrorPageGenerator
{
    private const string SharedCss = """
        <style>
            body {
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                background-color: #f8f9fa;
                color: #343a40;
            }

            .container {
                max-width: 80vw;
                margin: 20px auto;
                text-align: center;
                background: #ffffff;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            }

            h1 {
                font-size: 2.5rem;
                color: #dc3545;
                margin-bottom: 15px;
                border-bottom: 2px solid #dee2e6;
                padding-bottom: 10px;
            }

            h2 {
                font-size: 1.25rem;
                color: #6c757d;
                margin-bottom: 20px;
            }

            .key-value {
                display: table;
                width: 100%;
                max-width: 100%;
                border-collapse: collapse;
                table-layout: fixed;
                overflow: hidden;
                word-wrap: break-word;
            }

            .table-row {
                display: table-row;
                border-bottom: 1px solid #dee2e6;
            }

            .table-row:last-child {
                border-bottom: none;
            }

            .key,
            .value {
                display: table-cell;
                padding: 10px;
                vertical-align: top;
                overflow: auto;
            }

            .key {
                text-align: right;
                font-weight: bold;
                width: 30%;
                background: #ffffff;
            }

            .value {
                text-align: left;
                word-wrap: break-word;
                overflow-wrap: break-word;
                white-space: normal;
                background: #ffffff;
            }

            .key-value div:nth-child(odd) .key,
            .key-value div:nth-child(even) .value {
                background: #f1f3f5;
            }

            .key-value div:hover .key,
            .key-value div:hover .value {
                background: #bde0fe;
            }

            span {
                word-wrap: break-word;
                overflow-wrap: break-word;
            }

            code {
                word-wrap: break-word;
                overflow-wrap: break-word;
            }
        </style>
        """;

    private const string SharedTemplate = """
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>[title]</title>
            {0}
        </head>
        <body>
            [content]
        </body>
        </html>
        """;

    /// <summary>
    /// Generates a safe HTML error page with proper encoding.
    /// </summary>
    public static string Generate(
        int statusCode,
        string title,
        string message,
        string? friendlyMessage,
        string? stackTrace,
        Dictionary<string, object?>? data,
        ErrorHandlingOptions options,
        IHostEnvironment environment)
    {
        var sb = new StringBuilder("<div class=\"container\">");
        
        // Status code and title (always encoded)
        var statusMessage = StatusCodeMessageDictionary.GetStatusCodeMessage(statusCode);
        sb.AppendLine($"<h1>{WebUtility.HtmlEncode($"{statusCode} {statusMessage}")}</h1>");

        // Message (always encoded)
        var displayMessage = options.IsFriendlyErrors && !string.IsNullOrEmpty(friendlyMessage)
            ? friendlyMessage
            : message;
        sb.AppendLine($"<h2>{WebUtility.HtmlEncode(displayMessage)}</h2>");

        // Detailed information only in development or when explicitly enabled
        if (environment.IsDevelopment() || options.IncludeExceptionDetails)
        {
            if (!string.IsNullOrEmpty(stackTrace))
            {
                sb.AppendLine($"<div style=\"text-align: left; width: 100%; padding: 10px;\"><code>{WebUtility.HtmlEncode(stackTrace)}</code></div>");
            }

            sb.AppendLine("<div class=\"key-value\">");
            sb.Append($"""
                    <div class="table-row">
                        <span class="key">Message</span>
                        <span class="value">{WebUtility.HtmlEncode(message)}</span>
                    </div>
                    """);

            if (data != null)
            {
                foreach (var keyValue in data)
                {
                    var key = WebUtility.HtmlEncode(keyValue.Key ?? "null");
                    var value = SafeParseData(keyValue.Value, options);
                    sb.Append($"""
                            <div class="table-row">
                                <span class="key">{key}</span>
                                <span class="value">{value}</span>
                            </div>
                            """);
                }
            }

            sb.Append("</div>");
        }

        sb.Append("</div>");

        var content = sb.ToString();
        var template = string.Format(SharedTemplate, SharedCss);
        return template
            .Replace("[title]", WebUtility.HtmlEncode(title))
            .Replace("[content]", content);
    }

    private static string SafeParseData(object? obj, ErrorHandlingOptions options)
    {
        try
        {
            if (obj == null)
                return WebUtility.HtmlEncode("NULL");

            if (obj is string str)
            {
                return WebUtility.HtmlEncode(Truncate(str, options.MaxFieldLength));
            }

            if (obj is System.Collections.IDictionary dict)
            {
                var sb = new StringBuilder();
                var count = 0;
                foreach (var key in dict.Keys)
                {
                    if (count >= options.MaxCollectionItems)
                    {
                        sb.Append(WebUtility.HtmlEncode($"[{dict.Count - options.MaxCollectionItems} more items...]"));
                        break;
                    }
                    var keyStr = WebUtility.HtmlEncode(key?.ToString() ?? "null");
                    var valueStr = WebUtility.HtmlEncode(dict[key]?.ToString() ?? "null");
                    sb.Append($"{keyStr}: {valueStr}<br />");
                    count++;
                }
                return sb.ToString();
            }

            if (obj is System.Collections.IEnumerable enumerable && !(obj is string))
            {
                var sb = new StringBuilder();
                var count = 0;
                foreach (var item in enumerable)
                {
                    if (count >= options.MaxCollectionItems)
                    {
                        sb.Append(WebUtility.HtmlEncode("[... more items]"));
                        break;
                    }
                    sb.Append($"{WebUtility.HtmlEncode(item?.ToString() ?? "null")}<br />");
                    count++;
                }
                return sb.ToString();
            }

            var objStr = obj.ToString() ?? "NULL";
            return WebUtility.HtmlEncode(Truncate(objStr, options.MaxFieldLength));
        }
        catch
        {
            return WebUtility.HtmlEncode("[Error while collecting data]");
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value ?? string.Empty;

        return value.Substring(0, maxLength) + "... [TRUNCATED]";
    }
}
