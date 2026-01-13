using System.Buffers;
using Microsoft.Extensions.Logging;

namespace Eleon.Mcp.Infrastructure.Streaming;

public sealed class StreamPipe(ILogger<StreamPipe> logger)
{
    public async Task PipeAsync(
        Stream source,
        Stream destination,
        bool completeDestinationOnCompletion,
        string description,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        var buffer = ArrayPool<byte>.Shared.Rent(32 * 1024);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int read;
                try
                {
                    read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (IOException ioEx)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        logger.LogWarning(ioEx, "Failed to read bytes for {Direction}.", description);
                    }

                    break;
                }

                if (read == 0)
                {
                    break;
                }

                try
                {
                    await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
                    await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (IOException ioEx)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        logger.LogWarning(ioEx, "Failed to write bytes for {Direction}.", description);
                    }

                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            if (completeDestinationOnCompletion)
            {
                try
                {
                    destination.Dispose();
                }
                catch (Exception ex)
                {
                    logger.LogDebug(ex, "Failed to dispose destination for {Direction}.", description);
                }
            }
        }
    }
}

