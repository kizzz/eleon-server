using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using Eleon.Mcp.Abstractions;
using Eleon.McpSshGateway.Module.Application.Contracts.Dtos;
using Eleon.McpSshGateway.Module.Application.Contracts.Services;
using Eleon.McpSshGateway.Module.Domain.Entities;
using Eleon.McpSshGateway.Module.Domain.Repositories;
using Eleon.McpSshGateway.Module.Domain.ValueObjects;
using Eleon.McpSshGateway.Module;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;
using Xunit;
using Xunit.Sdk;

namespace Eleon.McpSshGateway.Module.Test.Integration;

[Trait("Category", "Integration")]
public sealed class SftpIntegrationTests
{
    private const string HostId = "network-protected-sftp";
    private const string HostName = "s.network-protected.com";
    private const int HostPort = 55;
    private const string Username = "eleon";
    private static readonly TimeSpan SftpTimeout = TimeSpan.FromSeconds(45);

    [SkippableFact]
    public async Task SshCommandAppService_Executes_WhoAmI_On_Sftp_Target()
    {
        var host = BuildHostOrSkip();
        EnsureEndpointReachable(host);
        using var provider = BuildServiceProvider(host);

        var service = provider.GetRequiredService<ISshCommandAppService>();
        var result = await service.ExecuteAsync(new ExecuteCommandInput
        {
            HostId = host.Id,
            Command = "whoami",
            TimeoutSeconds = 30
        }, CancellationToken.None);

        result.ExitCode.Should().Be(0);
        result.Stdout.Should().ContainEquivalentOf(Username);
    }

    [SkippableFact]
    public async Task SftpCli_List_Home_Directory_Via_Mcp_Host_Metadata()
    {
        var host = BuildHostOrSkip();
        EnsureEndpointReachable(host);
        using var provider = BuildServiceProvider(host);

        var dispatcher = provider.GetRequiredService<IMcpDispatcher>();
        var describeArgs = new JsonObject { ["hostId"] = host.Id };
        var describeResult = await dispatcher.DispatchAsync("ssh.describeHost", describeArgs, CancellationToken.None);

        if (describeResult.Value is null)
        {
            throw new XunitException("ssh.describeHost returned an empty payload.");
        }

        var hostDetails = describeResult.Value.Deserialize<HostDetailsDto>(McpJsonSerializer.Default)
                         ?? throw new XunitException("Failed to deserialize host metadata.");

        using var tempDir = new TempDirectory();
        var uploadPath = Path.Combine(tempDir.Path, "upload.txt");
        var remoteFile = $"mcp-upload-{Guid.NewGuid():N}.txt";
        var downloadPath = Path.Combine(tempDir.Path, "download.txt");
        var payload = $"eleon mcp sftp payload {Guid.NewGuid():N}";
        await File.WriteAllTextAsync(uploadPath, payload, CancellationToken.None).ConfigureAwait(false);

        var processResult = await RunSftpBatchAsync(
            hostDetails.Username,
            hostDetails.HostName,
            hostDetails.Port,
            host.Credential,
            new[] { "pwd", "ls" },
            SftpTimeout,
            CancellationToken.None,
            uploads: new[] { new SftpUpload(uploadPath, remoteFile) },
            downloads: new[] { new SftpDownload(remoteFile, downloadPath) },
            finalCommands: new[] { $"rm {remoteFile}" });

        processResult.ExitCode.Should().Be(0, processResult.StandardError);
        processResult.StandardOutput.Should().Contain("Documents");
        processResult.StandardOutput.Should().Contain("Downloads");
        File.Exists(downloadPath).Should().BeTrue();
        (await File.ReadAllTextAsync(downloadPath, CancellationToken.None).ConfigureAwait(false))
            .Should().Be(payload);
    }

    private static SshHost BuildHostOrSkip()
    {
        SshIntegrationTestSettings.EnsureEnabled("SFTP");
        var identityPath = ResolveIdentityPath();
        if (identityPath is null)
        {
            SkipTest("Set ELEON_MCP_SFTP_KEY_PATH or place id_ed25519 under %USERPROFILE%/.ssh to run SFTP tests.");
            return default!;
        }

        return new SshHost(
            HostId,
            "Network Protected SFTP",
            HostName,
            HostPort,
            Username,
            SshCredential.PrivateKeyCredential(identityPath),
            allowPatterns: new[] { "*" });
    }

    private static void EnsureEndpointReachable(SshHost host)
    {
        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(host.HostName, host.Port);
            if (!connectTask.Wait(TimeSpan.FromSeconds(5)))
            {
                SkipTest($"Timed out reaching {host.HostName}:{host.Port}.");
                return;
            }
        }
        catch (Exception ex) when (ex is SocketException or AggregateException or InvalidOperationException)
        {
            SkipTest($"Unable to reach {host.HostName}:{host.Port} ({ex.GetBaseException().Message}).");
        }
    }

    private static ServiceProvider BuildServiceProvider(SshHost host)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddProvider(NullLoggerProvider.Instance));
        services.AddApplicationAsync<McpSshGatewayModuleCollector>().GetAwaiter().GetResult();
        services.AddSingleton<IHostRepository>(_ => new StaticHostRepository(host));
        services.AddSingleton<ICommandAuditRepository, InMemoryAuditRepository>();
        return services.BuildServiceProvider();
    }

    private static async Task<ProcessExecutionResult> RunSftpBatchAsync(
        string username,
        string hostName,
        int port,
        SshCredential credential,
        IEnumerable<string>? commands,
        TimeSpan timeout,
        CancellationToken cancellationToken,
        IEnumerable<SftpUpload>? uploads = null,
        IEnumerable<SftpDownload>? downloads = null,
        IEnumerable<string>? finalCommands = null)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "sftp",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        psi.ArgumentList.Add("-o");
        psi.ArgumentList.Add("BatchMode=yes");
        psi.ArgumentList.Add("-o");
        psi.ArgumentList.Add("StrictHostKeyChecking=no");
        psi.ArgumentList.Add("-o");
        psi.ArgumentList.Add($"UserKnownHostsFile={(OperatingSystem.IsWindows() ? "NUL" : "/dev/null")}");

        if (credential.Type == SshCredentialType.PrivateKey && !string.IsNullOrWhiteSpace(credential.PrivateKeyPath))
        {
            psi.ArgumentList.Add("-i");
            psi.ArgumentList.Add(credential.PrivateKeyPath);
        }

        psi.ArgumentList.Add("-P");
        psi.ArgumentList.Add(port.ToString(CultureInfo.InvariantCulture));
        psi.ArgumentList.Add("-b");
        psi.ArgumentList.Add("-");
        psi.ArgumentList.Add($"{username}@{hostName}");

        var process = new Process { StartInfo = psi };
        try
        {
            if (!process.Start())
            {
                throw new InvalidOperationException("Failed to start sftp process.");
            }
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 2)
        {
            SkipTest("sftp CLI is not available. Install the OpenSSH client to run this test.");
            return new ProcessExecutionResult(-1, string.Empty, ex.Message);
        }

        var stdin = process.StandardInput;
        await WriteCommandsAsync(stdin, commands).ConfigureAwait(false);

        if (uploads is not null)
        {
            foreach (var upload in uploads)
            {
                if (!File.Exists(upload.LocalPath))
                {
                    throw new FileNotFoundException($"Upload source does not exist: {upload.LocalPath}", upload.LocalPath);
                }

                var quotedLocal = QuotePath(upload.LocalPath);
                var quotedRemote = QuotePath(upload.RemotePath);
                await stdin.WriteLineAsync($"put {quotedLocal} {quotedRemote}").ConfigureAwait(false);
            }
        }

        if (downloads is not null)
        {
            foreach (var download in downloads)
            {
                var directory = Path.GetDirectoryName(download.LocalPath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var quotedRemote = QuotePath(download.RemotePath);
                var quotedLocal = QuotePath(download.LocalPath);
                await stdin.WriteLineAsync($"get {quotedRemote} {quotedLocal}").ConfigureAwait(false);
            }
        }

        await WriteCommandsAsync(stdin, finalCommands).ConfigureAwait(false);
        await stdin.WriteLineAsync("bye").ConfigureAwait(false);

        stdin.Close();
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);
        try
        {
            await process.WaitForExitAsync(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            TryKill(process);
            throw new TimeoutException("sftp command timed out.");
        }

        var stdout = await stdoutTask.ConfigureAwait(false);
        var stderr = await stderrTask.ConfigureAwait(false);
        return new ProcessExecutionResult(process.ExitCode, stdout, stderr);

        static async Task WriteCommandsAsync(StreamWriter writer, IEnumerable<string>? commandsToWrite)
        {
            if (commandsToWrite is null)
            {
                return;
            }

            foreach (var command in commandsToWrite)
            {
                await writer.WriteLineAsync(command).ConfigureAwait(false);
            }
        }

        static string QuotePath(string path) =>
            string.IsNullOrWhiteSpace(path) || path.IndexOfAny(new[] { ' ', '\t' }) < 0
                ? path
                : $"\"{path}\"";
    }

    private static void TryKill(Process process)
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
        }
    }

    private static string? ResolveIdentityPath()
    {
        var overridePath = Environment.GetEnvironmentVariable("ELEON_MCP_SFTP_KEY_PATH");
        if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
        {
            return Path.GetFullPath(overridePath);
        }

        var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh", "id_ed25519");
        return File.Exists(defaultPath) ? defaultPath : null;
    }

    private static void SkipTest(string message)
    {
        Console.WriteLine($"[SFTP-SKIP] {message}");
        Skip.If(true, message);
        throw new InvalidOperationException("Skip.If did not abort the test.");
    }

    private sealed record ProcessExecutionResult(int ExitCode, string StandardOutput, string StandardError);
    private sealed record SftpUpload(string LocalPath, string RemotePath);
    private sealed record SftpDownload(string RemotePath, string LocalPath);

    private sealed class StaticHostRepository : IHostRepository
    {
        private readonly SshHost host;

        public StaticHostRepository(SshHost host)
        {
            this.host = host;
        }

        public Task<SshHost?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(string.Equals(host.Id, id, StringComparison.OrdinalIgnoreCase) ? host : null);
        }

        public Task<IReadOnlyList<SshHost>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<SshHost>>(new[] { host });
        }
    }

    private sealed class InMemoryAuditRepository : ICommandAuditRepository
    {
        private readonly List<CommandAudit> audits = new();

        public IReadOnlyList<CommandAudit> Audits => audits;

        public Task AddAsync(CommandAudit audit, CancellationToken cancellationToken = default)
        {
            audits.Add(audit);
            return Task.CompletedTask;
        }
    }

    private sealed class TempDirectory : IDisposable
    {
        public TempDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"mcp-sftp-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, recursive: true);
                }
            }
            catch
            {
            }
        }
    }
}
