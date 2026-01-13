using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Eleon.Ssh;

public sealed class SshNetCommandRunner : ISshCommandRunner
{
    private readonly ILogger<SshNetCommandRunner> logger;

    public SshNetCommandRunner(ILogger<SshNetCommandRunner>? logger = null)
    {
        this.logger = logger ?? NullLogger<SshNetCommandRunner>.Instance;
    }

    public async Task<SshCommandResult> RunAsync(SshConnectionInfo connection, string command, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (string.IsNullOrWhiteSpace(command))
        {
            throw new ArgumentException("Command is required", nameof(command));
        }

        if (connection.AuthenticationMode == SshAuthenticationMode.Agent)
        {
            return await ExecuteWithAgentAsync(connection, command, timeout, cancellationToken).ConfigureAwait(false);
        }

        return await Task.Run(() => Execute(connection, command, timeout, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private SshCommandResult Execute(SshConnectionInfo connection, string command, TimeSpan timeout, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var client = CreateClient(connection, timeout);
        var commandTimeout = NormalizeTimeout(timeout);

        try
        {
            client.Connect();
        }
        catch (Exception ex) when (ex is SshException or SocketException)
        {
            throw new SshCommandException($"Failed to connect to {connection.Host}:{connection.Port}", ex);
        }

        SshCommand? sshCommand = null;
        try
        {
            sshCommand = client.CreateCommand(command);
            sshCommand.CommandTimeout = commandTimeout;
            using var registration = cancellationToken.Register(() => sshCommand?.CancelAsync());
            cancellationToken.ThrowIfCancellationRequested();

            var stopwatch = Stopwatch.StartNew();
            var stdout = sshCommand.Execute();
            stopwatch.Stop();

            var exitCode = sshCommand.ExitStatus;
            return new SshCommandResult(exitCode, stdout, sshCommand.Error, stopwatch.Elapsed);
        }
        catch (SshOperationTimeoutException ex)
        {
            throw new TimeoutException($"SSH command timed out after {commandTimeout.TotalSeconds:N0}s", ex);
        }
        catch (SshException ex)
        {
            logger.LogWarning(ex, "SSH command failed");
            throw new SshCommandException(ex.Message, ex);
        }
        finally
        {
            sshCommand?.Dispose();
            if (client.IsConnected)
            {
                client.Disconnect();
            }
        }
    }

    private static SshClient CreateClient(SshConnectionInfo connection, TimeSpan timeout)
    {
        var connectionInfo = BuildConnectionInfo(connection, timeout);
        return new SshClient(connectionInfo)
        {
            KeepAliveInterval = connection.KeepAliveInterval
        };
    }

    private static ConnectionInfo BuildConnectionInfo(SshConnectionInfo connection, TimeSpan timeout)
    {
        if (string.IsNullOrWhiteSpace(connection.Host))
        {
            throw new ArgumentException("Host is required", nameof(connection));
        }

        if (string.IsNullOrWhiteSpace(connection.Username))
        {
            throw new ArgumentException("Username is required", nameof(connection));
        }

        var methods = new List<AuthenticationMethod>
        {
            connection.AuthenticationMode switch
            {
                SshAuthenticationMode.Password => BuildPasswordAuth(connection),
                SshAuthenticationMode.PrivateKey => BuildPrivateKeyAuth(connection),
                SshAuthenticationMode.Agent => throw new InvalidOperationException("Agent authentication is executed through the system SSH client."),
                _ => throw new ArgumentOutOfRangeException(nameof(connection), "Unsupported authentication mode")
            }
        };

        var info = new ConnectionInfo(connection.Host, connection.Port, connection.Username, methods.ToArray())
        {
            Timeout = NormalizeTimeout(timeout)
        };

        return info;
    }

    private static AuthenticationMethod BuildPasswordAuth(SshConnectionInfo connection)
    {
        if (string.IsNullOrEmpty(connection.Password))
        {
            throw new ArgumentException("Password is required for password authentication", nameof(connection));
        }

        return new PasswordAuthenticationMethod(connection.Username, connection.Password);
    }

    private static AuthenticationMethod BuildPrivateKeyAuth(SshConnectionInfo connection)
    {
        if (string.IsNullOrWhiteSpace(connection.PrivateKeyPath))
        {
            throw new ArgumentException("PrivateKeyPath is required for private key authentication", nameof(connection));
        }

        var keyFile = string.IsNullOrEmpty(connection.PrivateKeyPassphrase)
            ? new PrivateKeyFile(connection.PrivateKeyPath)
            : new PrivateKeyFile(connection.PrivateKeyPath, connection.PrivateKeyPassphrase);

        return new PrivateKeyAuthenticationMethod(connection.Username, keyFile);
    }

    private static TimeSpan NormalizeTimeout(TimeSpan timeout)
    {
        return timeout <= TimeSpan.Zero ? TimeSpan.FromSeconds(60) : timeout;
    }

    private async Task<SshCommandResult> ExecuteWithAgentAsync(
        SshConnectionInfo connection,
        string command,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        var sshExecutable = string.IsNullOrWhiteSpace(connection.AgentExecutable) ? "ssh" : connection.AgentExecutable!;
        var agentSocket = Environment.GetEnvironmentVariable("SSH_AUTH_SOCK");
        if (string.IsNullOrWhiteSpace(agentSocket))
        {
            throw new SshCommandException("SSH agent authentication requested but SSH_AUTH_SOCK is not set. Start ssh-agent and add keys before using agent mode.");
        }

        var psi = new ProcessStartInfo
        {
            FileName = sshExecutable,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false,
            UseShellExecute = false
        };

        psi.Environment["SSH_AUTH_SOCK"] = agentSocket;
        var agentPid = Environment.GetEnvironmentVariable("SSH_AGENT_PID");
        if (!string.IsNullOrWhiteSpace(agentPid))
        {
            psi.Environment["SSH_AGENT_PID"] = agentPid;
        }

        psi.ArgumentList.Add("-o");
        psi.ArgumentList.Add("BatchMode=yes");
        psi.ArgumentList.Add("-o");
        psi.ArgumentList.Add("StrictHostKeyChecking=no");
        psi.ArgumentList.Add("-o");
        var knownHostsTarget = OperatingSystem.IsWindows() ? "NUL" : "/dev/null";
        psi.ArgumentList.Add("-o");
        psi.ArgumentList.Add($"UserKnownHostsFile={knownHostsTarget}");
        psi.ArgumentList.Add("-p");
        psi.ArgumentList.Add(connection.Port.ToString(CultureInfo.InvariantCulture));

        if (timeout > TimeSpan.Zero)
        {
            var connectTimeout = (int)Math.Clamp(Math.Ceiling(timeout.TotalSeconds), 1, 120);
            psi.ArgumentList.Add("-o");
            psi.ArgumentList.Add($"ConnectTimeout={connectTimeout}");
        }

        psi.ArgumentList.Add($"{connection.Username}@{connection.Host}");
        psi.ArgumentList.Add(command);

        using var process = new Process { StartInfo = psi };
        try
        {
            if (!process.Start())
            {
                throw new SshCommandException($"Failed to start '{sshExecutable}'.");
            }
        }
        catch (Win32Exception ex)
        {
            throw new SshCommandException($"SSH agent execution failed. Ensure '{sshExecutable}' is installed and available.", ex);
        }

        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();
        var stopwatch = Stopwatch.StartNew();

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (timeout > TimeSpan.Zero)
        {
            linkedCts.CancelAfter(timeout);
        }

        try
        {
            await process.WaitForExitAsync(linkedCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            TryTerminate(process);
            throw new TimeoutException($"SSH command timed out after {timeout.TotalSeconds:N0}s");
        }

        stopwatch.Stop();
        var stdout = await stdoutTask.ConfigureAwait(false);
        var stderr = await stderrTask.ConfigureAwait(false);
        return new SshCommandResult(process.ExitCode, stdout, stderr, stopwatch.Elapsed);
    }

    private static void TryTerminate(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch
        {
            // ignored
        }
    }
}
