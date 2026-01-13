using System.Text;

namespace Eleon.McpGateway.Module.Test.HttpApi.TestHelpers;

/// <summary>
/// Helper to parse SSE events reliably.
/// </summary>
internal static class SseEventReader
{
    public static async Task<(string EventType, string Data)> ReadNextEventAsync(StreamReader reader, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        string? eventType = null;
        string? data = null;

        while (!cts.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
            {
                break;
            }

            if (line.StartsWith("event:", StringComparison.Ordinal))
            {
                eventType = line.Substring(6).Trim();
            }
            else if (line.StartsWith("data:", StringComparison.Ordinal))
            {
                data = line.Substring(5).Trim();
            }
            else if (line.Length == 0 && eventType != null && data != null)
            {
                return (eventType, data);
            }
        }

        throw new TimeoutException("No SSE event received in allotted time.");
    }

    public static async Task<List<(string EventType, string Data)>> ReadAllEventsAsync(StreamReader reader, int count, TimeSpan timeout)
    {
        var events = new List<(string EventType, string Data)>();
        for (int i = 0; i < count; i++)
        {
            var evt = await ReadNextEventAsync(reader, timeout);
            events.Add(evt);
        }
        return events;
    }

    public static async Task<bool> WaitForKeepaliveAsync(StreamReader reader, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        while (!cts.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
            {
                return false;
            }

            if (line.StartsWith(":", StringComparison.Ordinal))
            {
                return true;
            }
        }
        return false;
    }
}
