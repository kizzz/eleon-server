using Logging.Module.ErrorHandling.Options;
using System.Text;

namespace Logging.Module.ErrorHandling.Helpers;

/// <summary>
/// Safely collects exception details with bounded recursion and size limits.
/// </summary>
public static class SafeExceptionCollector
{
    /// <summary>
    /// Collects exception details with depth and size limits.
    /// </summary>
    public static (string Message, string StackTrace, Dictionary<string, object?> Data) Collect(
        Exception exception,
        ErrorHandlingOptions options)
    {
        if (exception == null)
            return (string.Empty, string.Empty, new Dictionary<string, object?>());

        var messageBuilder = new StringBuilder();
        var stackTraceBuilder = new StringBuilder();
        var data = new Dictionary<string, object?>();

        CollectRecursive(exception, options, messageBuilder, stackTraceBuilder, data, depth: 0);

        return (
            Truncate(messageBuilder.ToString(), options.MaxFieldLength),
            Truncate(stackTraceBuilder.ToString(), options.MaxFieldLength * 2), // Stack traces can be longer
            data
        );
    }

    private static void CollectRecursive(
        Exception exception,
        ErrorHandlingOptions options,
        StringBuilder messageBuilder,
        StringBuilder stackTraceBuilder,
        Dictionary<string, object?> data,
        int depth)
    {
        if (exception == null || depth >= options.MaxInnerExceptionDepth)
        {
            if (depth >= options.MaxInnerExceptionDepth)
            {
                messageBuilder.Append(" [Inner exception chain truncated]");
            }
            return;
        }

        // Build message chain
        if (depth > 0)
        {
            messageBuilder.Append(" => ");
        }
        messageBuilder.Append(exception.Message);

        // Collect stack trace
        stackTraceBuilder
            .Append(exception.GetType().Name)
            .Append(": ")
            .Append(exception.Message)
            .AppendLine()
            .Append(exception.StackTrace)
            .AppendLine();

        // Collect data (only from first exception to avoid duplicates)
        if (depth == 0)
        {
            CollectData(exception, data, options);
        }

        // Recurse for inner exception
        if (exception.InnerException != null)
        {
            CollectRecursive(exception.InnerException, options, messageBuilder, stackTraceBuilder, data, depth + 1);
        }
    }

    private static void CollectData(Exception exception, Dictionary<string, object?> data, ErrorHandlingOptions options)
    {
        if (exception.Data == null)
            return;

        var itemCount = 0;
        foreach (var key in exception.Data.Keys)
        {
            if (itemCount >= options.MaxCollectionItems)
            {
                data["[TRUNCATED]"] = $"{exception.Data.Count - options.MaxCollectionItems} more items...";
                break;
            }

            if (key == null)
                continue;

            try
            {
                var keyStr = key.ToString() ?? "null";
                var value = exception.Data[key];
                
                // Safely stringify value
                var valueStr = SafeStringify(value, options);
                data[keyStr] = valueStr;
                itemCount++;
            }
            catch
            {
                // Skip problematic entries
                continue;
            }
        }
    }

    private static string SafeStringify(object? value, ErrorHandlingOptions options)
    {
        if (value == null)
            return "NULL";

        try
        {
            var str = value.ToString() ?? "NULL";
            return Truncate(str, options.MaxFieldLength);
        }
        catch
        {
            return "[Error stringifying value]";
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value ?? string.Empty;

        return value.Substring(0, maxLength) + "... [TRUNCATED]";
    }
}
