using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Eleon.McpGateway.Module.Domain;
using Eleon.McpGateway.Module.Infrastructure.Configuration;
using Eleon.Mcp.Infrastructure.Processes;
using Microsoft.Extensions.Logging;

namespace Eleon.McpGateway.Module.Infrastructure.Backends;

public sealed class SshMcpBackend(
    SshBackendSettings settings,
    ILogger<SshMcpBackend> logger) : IMcpBackend
{
    private readonly SemaphoreSlim sendLock = new(1, 1);
    private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);
    private Process? process;
    private StreamWriter? inputWriter;
    private Channel<JsonNode>? outboundChannel;
    private CancellationTokenSource? pumpCts;
    private bool started;

    public string Name => "ssh-stdio";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (started)
        {
            return Task.CompletedTask;
        }

        started = true;
        outboundChannel = Channel.CreateUnbounded<JsonNode>(new UnboundedChannelOptions
        {
            SingleWriter = true,
            SingleReader = true
        });

        var startInfo = BuildStartInfo();
        logger.LogInformation("Launching backend {Backend} via {Command}.", Name, ProcessLogFormatter.Format(startInfo));
        process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        try
        {
            process.Start();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed to start backend process for {Backend}.", Name);
            outboundChannel.Writer.TryComplete(ex);
            throw;
        }

        process.Exited += (_, _) =>
        {
            logger.LogWarning("Backend {Backend} exited with code {ExitCode}.", Name, process.ExitCode);
            outboundChannel.Writer.TryComplete();
            pumpCts?.Cancel();
        };

        inputWriter = new StreamWriter(process.StandardInput.BaseStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
        {
            AutoFlush = true
        };

        pumpCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _ = Task.Run(() => PumpStdOutAsync(process.StandardOutput, outboundChannel.Writer, pumpCts.Token), CancellationToken.None);
        _ = Task.Run(() => PumpStdErrAsync(process.StandardError, pumpCts.Token), CancellationToken.None);

        return Task.CompletedTask;
    }

    public async Task SendAsync(JsonNode message, CancellationToken cancellationToken)
    {
        if (inputWriter is null)
        {
            throw new InvalidOperationException("Backend is not ready.");
        }

        var payload = message.ToJsonString(serializerOptions);
        await sendLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await inputWriter.WriteLineAsync(payload).ConfigureAwait(false);
        }
        finally
        {
            sendLock.Release();
        }
    }

    public IAsyncEnumerable<JsonNode> ReceiveAsync(CancellationToken cancellationToken)
    {
        if (outboundChannel is null)
        {
            throw new InvalidOperationException("Backend is not ready.");
        }

        return outboundChannel.Reader.ReadAllAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        pumpCts?.Cancel();

        if (process is { HasExited: false })
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Failed to terminate backend process {Backend}.", Name);
            }
        }

        if (process is not null)
        {
            await process.WaitForExitAsync().ConfigureAwait(false);
            process.Dispose();
        }

        inputWriter?.Dispose();
        pumpCts?.Dispose();
        sendLock.Dispose();
        outboundChannel?.Writer.TryComplete();
    }

    private ProcessStartInfo BuildStartInfo()
    {
        var startInfo = new ProcessStartInfo
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = settings.WorkingDirectory
        };

        if (Path.GetExtension(settings.ExecutablePath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
        {
            startInfo.FileName = "dotnet";
            startInfo.ArgumentList.Add(settings.ExecutablePath);
        }
        else
        {
            startInfo.FileName = settings.ExecutablePath;
        }

        foreach (var arg in settings.AdditionalArguments)
        {
            startInfo.ArgumentList.Add(arg);
        }

        return startInfo;
    }

    private async Task PumpStdOutAsync(StreamReader reader, ChannelWriter<JsonNode> writer, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync().ConfigureAwait(false);
                if (line is null)
                {
                    break;
                }

                if (!LooksLikeJson(line))
                {
                    logger.LogDebug("Backend {Backend} stdout: {Message}", Name, line);
                    continue;
                }

                try
                {
                    var node = JsonNode.Parse(line);
                    if (node is not null)
                    {
                        await writer.WriteAsync(node, cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (JsonException ex)
                {
                    logger.LogWarning(ex, "Backend {Backend} emitted invalid JSON: {Payload}", Name, line);
                }
            }
        }
        finally
        {
            writer.TryComplete();
        }
    }

    private static bool LooksLikeJson(string line)
    {
        var trimmed = line.TrimStart();
        return trimmed.StartsWith('{') || trimmed.StartsWith('[');
    }

    private async Task PumpStdErrAsync(StreamReader reader, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync().ConfigureAwait(false);
                if (line is null)
                {
                    break;
                }

                logger.LogInformation("[{Backend}] {Message}", Name, line);
            }
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
    }
}

