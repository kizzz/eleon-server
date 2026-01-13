using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ReliabilityOptions = SharedModule.modules.Otel.Module.OtelOptions.ReliabilityOptions;

namespace SharedModule.modules.Otel.Module;

/// <summary>
/// Resolves OTLP headers from configuration (supports Dictionary or raw OTLP string format).
/// Includes redaction helpers to ensure headers are never logged.
/// </summary>
internal static class OtlpHeadersResolver
{
  /// <summary>
  /// Resolves headers from reliability options and returns them as a dictionary.
  /// Supports both Dictionary format and raw OTLP string format ("k1=v1,k2=v2").
  /// </summary>
  public static Dictionary<string, string>? ResolveHeaders(ReliabilityOptions.HeadersOptions? headersOptions, ILogger? logger = null)
  {
    if (headersOptions == null)
      return null;

    var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    // Process Dictionary format
    if (headersOptions.Dictionary != null && headersOptions.Dictionary.Count > 0)
    {
      foreach (var kvp in headersOptions.Dictionary)
      {
        if (!string.IsNullOrWhiteSpace(kvp.Key))
        {
          result[kvp.Key] = kvp.Value ?? string.Empty;
        }
      }
    }

    // Process raw string format (e.g., "k1=v1,k2=v2")
    if (!string.IsNullOrWhiteSpace(headersOptions.Raw))
    {
      var rawParts = headersOptions.Raw.Split(',', StringSplitOptions.RemoveEmptyEntries);
      foreach (var part in rawParts)
      {
        var kvp = part.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
        if (kvp.Length == 2)
        {
          var key = kvp[0].Trim();
          var value = kvp[1].Trim();
          if (!string.IsNullOrWhiteSpace(key))
          {
            result[key] = value;
          }
        }
        else if (kvp.Length == 1)
        {
          // Key without value
          var key = kvp[0].Trim();
          if (!string.IsNullOrWhiteSpace(key))
          {
            result[key] = string.Empty;
          }
        }
      }
    }

    return result.Count > 0 ? result : null;
  }

  /// <summary>
  /// Logs header key names only (never values) for debugging.
  /// </summary>
  public static void LogHeaderKeys(Dictionary<string, string>? headers, ILogger logger, string context)
  {
    if (headers == null || headers.Count == 0)
      return;

    var keys = headers.Keys.OrderBy(k => k).ToArray();
    logger.LogDebug("{Context}: OTLP headers configured with {Count} keys: {Keys}", context, keys.Length, string.Join(", ", keys));
  }
}
