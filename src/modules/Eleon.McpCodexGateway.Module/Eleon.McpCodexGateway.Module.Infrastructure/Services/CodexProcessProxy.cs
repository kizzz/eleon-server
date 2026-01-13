using System.Diagnostics;
using Eleon.Mcp.Infrastructure.Streaming;
using Eleon.McpCodexGateway.Module.Application.Contracts.Services;
using Eleon.McpCodexGateway.Module.Application.Services;
using Microsoft.Extensions.Logging;

namespace Eleon.McpCodexGateway.Module.Infrastructure.Services;

public sealed class CodexProcessProxy(
    ICodexProcessOptionsProvider optionsProvider,
    StreamPipe streamPipe,
    ErrorStreamForwarder errorStreamForwarder,
    ILogger<CodexProcessProxy> logger)
{
    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        var options = optionsProvider.GetOptions();
        var startInfo = CodexCommandLineBuilder.CreateStartInfo(options);
        logger.LogInformation("Launching Codex CLI via: {CommandLine}", CodexCommandLineBuilder.FormatForLogging(startInfo));

        using var process = new Process
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
            logger.LogError(ex, "Failed to start the Codex CLI. Ensure that the 'codex' command is available in PATH.");
            return -1;
        }

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var linkedToken = linkedCts.Token;
        using var _ = cancellationToken.Register(() => RequestProcessTermination(process));

        var stdin = Console.OpenStandardInput();
        var stdout = Console.OpenStandardOutput();
        var stderr = Console.Error;

        var forwarders = new[]
        {
            streamPipe.PipeAsync(stdin, process.StandardInput.BaseStream, completeDestinationOnCompletion: true, "stdin->codex", linkedToken),
            streamPipe.PipeAsync(process.StandardOutput.BaseStream, stdout, completeDestinationOnCompletion: false, "codex->stdout", linkedToken),
            errorStreamForwarder.ForwardAsync(process.StandardError, stderr, "codex", linkedToken)
        };

        var exitCode = await WaitForProcessExitAsync(process, linkedToken);
        linkedCts.Cancel();
        await Task.WhenAll(forwarders);

        logger.LogInformation("Codex CLI exited with code {ExitCode}", exitCode);
        return exitCode;
    }

    private static async Task<int> WaitForProcessExitAsync(Process process, CancellationToken cancellationToken)
    {
        try
        {
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
            {
                RequestProcessTermination(process);
                await process.WaitForExitAsync().ConfigureAwait(false);
            }
        }

        return process.ExitCode;
    }

    private static void RequestProcessTermination(Process process)
    {
        if (process.HasExited)
        {
            return;
        }

        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch (Exception)
        {
            // ignored: process may already be terminating.
        }
    }
}

