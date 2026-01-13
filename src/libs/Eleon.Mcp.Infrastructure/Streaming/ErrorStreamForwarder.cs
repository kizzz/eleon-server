using Microsoft.Extensions.Logging;

namespace Eleon.Mcp.Infrastructure.Streaming;

public sealed class ErrorStreamForwarder(ILogger<ErrorStreamForwarder> logger)
{
    public async Task ForwardAsync(TextReader source, TextWriter destination, string prefix, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await source.ReadLineAsync().ConfigureAwait(false);
                if (line is null)
                {
                    break;
                }

                await destination.WriteLineAsync($"[{prefix}] {line}").ConfigureAwait(false);
                await destination.FlushAsync().ConfigureAwait(false);
            }
        }
        catch (ObjectDisposedException)
        {
            // stream disposed as part of shutdown.
        }
        catch (IOException ioEx)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ioEx, "Failed to forward stderr output.");
            }
        }
    }
}

