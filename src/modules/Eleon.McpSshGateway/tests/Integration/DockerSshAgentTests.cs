using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Eleon.McpSshGateway.Module.Application.Contracts.Dtos;
using Eleon.McpSshGateway.Module.Application.Contracts.Services;
using Eleon.McpSshGateway.Module.Infrastructure.Audit;
using Eleon.McpSshGateway.Module.Infrastructure.HostCatalog;
using Eleon.McpSshGateway.Module;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;
using Xunit;

namespace Eleon.McpSshGateway.Module.Test.Integration;

[Trait("Category", "Integration")]
public sealed class DockerSshAgentTests
{
    [SkippableFact]
    public async Task Executes_Command_Via_SshAgent_Against_Docker_Target()
    {
        SshIntegrationTestSettings.EnsureEnabled("Docker SSH agent");
        using var tempDir = new TempDirectory();
        if (!await EnsureCliAsync("ssh") ||
            !await EnsureCliAsync("ssh-agent") ||
            !await EnsureCliAsync("ssh-add") ||
            !await EnsureCliAsync("ssh-keygen") ||
            !await EnsureCliAsync("docker"))
        {
            return;
        }

        var privateKeyPath = Path.Combine(tempDir.Path, "id_ed25519");
        await RunProcessAsync("ssh-keygen", "-t", "ed25519", "-q", "-N", string.Empty, "-f", privateKeyPath);

        var containerName = await StartDockerContainerAsync();
        if (containerName is null)
        {
            return;
        }

        try
        {
            var hostPort = await GetDockerHostPortAsync(containerName);
            var agentContext = await StartAgentAsync(privateKeyPath);
            if (agentContext is null)
            {
                return;
            }

            await using var _agentDispose = agentContext;
            using var envScope = new EnvironmentScope(agentContext.EnvironmentVariables);

            await AuthorizeKeyAsync(containerName, privateKeyPath + ".pub");

            var catalogPath = Path.Combine(tempDir.Path, "hosts.json");
            var auditPath = Path.Combine(tempDir.Path, "audit.log");
            var hostId = "docker-agent";
            await WriteHostCatalogAsync(catalogPath, hostId, hostPort);

            using var provider = BuildServiceProvider(catalogPath, auditPath);
            var appService = provider.GetRequiredService<ISshCommandAppService>();

            var result = await appService.ExecuteAsync(new ExecuteCommandInput
            {
                HostId = hostId,
                Command = "whoami",
                TimeoutSeconds = 30
            }, CancellationToken.None);

            result.ExitCode.Should().Be(0);
            result.Stdout.Trim().Should().Be("root");
        }
        finally
        {
            await StopDockerContainerAsync(containerName);
        }
    }

    private static ServiceProvider BuildServiceProvider(string catalogPath, string auditPath)
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddProvider(NullLoggerProvider.Instance));
        services.AddApplicationAsync<McpSshGatewayModuleCollector>().GetAwaiter().GetResult();
        services.Configure<FileHostRepositoryOptions>(options => options.CatalogPath = catalogPath);
        services.Configure<FileCommandAuditRepositoryOptions>(options => options.AuditLogPath = auditPath);
        return services.BuildServiceProvider();
    }

    private static async Task<string?> StartDockerContainerAsync()
    {
        var name = $"mcp-ssh-{Guid.NewGuid():N}";
        var result = await RunProcessAsync("docker", "run", "-d", "--rm", "--name", name, "-P", "rastasheep/ubuntu-sshd:latest");
        if (result.ExitCode != 0)
        {
            Console.WriteLine("Skipping docker-backed test: {0}", result.StandardError.Trim());
            return null;
        }

        return name;
    }

    private static async Task StopDockerContainerAsync(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        await RunProcessAsync("docker", "stop", name);
    }

    private static async Task<int> GetDockerHostPortAsync(string containerName)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            var result = await RunProcessAsync("docker", "port", containerName, "22/tcp");
            if (result.ExitCode == 0 && TryParsePort(result.StandardOutput, out var port))
            {
                return port;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        throw new InvalidOperationException("Failed to resolve docker host port for ssh container.");
    }

    private static bool TryParsePort(string output, out int port)
    {
        var line = output.Split('\n', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        if (line is null)
        {
            port = 0;
            return false;
        }

        var colonIndex = line.LastIndexOf(':');
        if (colonIndex < 0)
        {
            port = 0;
            return false;
        }

        return int.TryParse(line[(colonIndex + 1)..].Trim(), out port);
    }

    private static async Task AuthorizeKeyAsync(string containerName, string publicKeyPath)
    {
        var publicKey = await File.ReadAllTextAsync(publicKeyPath);
        var escapedKey = publicKey.Trim().Replace("'", "'\"'\"'");
        var command = $"mkdir -p /root/.ssh && printf '%s\\n' '{escapedKey}' >> /root/.ssh/authorized_keys && chmod 600 /root/.ssh/authorized_keys";
        var result = await RunProcessAsync("docker", "exec", containerName, "/bin/sh", "-c", command);
        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException($"Failed to authorize public key inside container: {result.StandardError}");
        }
    }

    private static async Task WriteHostCatalogAsync(string catalogPath, string hostId, int port)
    {
        var doc = new
        {
            hosts = new[]
            {
                new
                {
                    id = hostId,
                    name = "Docker Agent Host",
                    hostname = "127.0.0.1",
                    port,
                    username = "root",
                    tags = new[] { "docker", "agent" },
                    allow = new[] { "whoami" },
                    deny = new[] { "rm *", "shutdown *" },
                    enabled = true,
                    auth = new { mode = "agent" }
                }
            }
        };

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };
        await File.WriteAllTextAsync(catalogPath, JsonSerializer.Serialize(doc, options));
    }

    private static async Task<AgentContext?> StartAgentAsync(string privateKeyPath)
    {
        try
        {
            var startResult = await RunProcessAsync("ssh-agent", "-s");
            if (startResult.ExitCode != 0)
            {
                Console.WriteLine("Skipping agent-backed test: ssh-agent failed: {0}", startResult.StandardError.Trim());
                return null;
            }

            var env = ParseAgentVariables(startResult.StandardOutput);
            if (env is null)
            {
                Console.WriteLine("Skipping agent-backed test: ssh-agent output missing SSH_AUTH_SOCK.");
                return null;
            }

            await RunProcessAsync("ssh-add", env, privateKeyPath);
            return new AgentContext(env);
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 2)
        {
            Console.WriteLine("Skipping agent-backed test: ssh-agent not available ({0})", ex.Message);
            return null;
        }
    }

    private static Dictionary<string, string>? ParseAgentVariables(string output)
    {
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var segment = line.Split(';', 2)[0];
            var kv = segment.Split('=', 2);
            if (kv.Length == 2)
            {
                dict[kv[0]] = kv[1];
            }
        }

        if (!dict.ContainsKey("SSH_AUTH_SOCK"))
        {
            return null;
        }

        return dict;
    }

    private static async Task<bool> EnsureCliAsync(string name)
    {
        try
        {
            await RunProcessAsync(name, "-V");
            return true;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 2)
        {
            Console.WriteLine("Skipping docker-backed test: {0} is not installed ({1})", name, ex.Message);
            return false;
        }
        catch
        {
            return true;
        }
    }

    private static Task<ProcessResult> RunProcessAsync(string fileName, params string[] arguments)
        => RunProcessAsync(fileName, arguments, null);

    private static Task<ProcessResult> RunProcessAsync(string fileName, IDictionary<string, string> environment, params string[] arguments)
        => RunProcessAsync(fileName, arguments, environment);

    private static async Task<ProcessResult> RunProcessAsync(string fileName, string[] arguments, IDictionary<string, string>? environment)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        foreach (var argument in arguments)
        {
            psi.ArgumentList.Add(argument);
        }

        if (environment is not null)
        {
            foreach (var (key, value) in environment)
            {
                psi.Environment[key] = value;
            }
        }

        using var process = new Process { StartInfo = psi };
        process.Start();
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        return new ProcessResult(process.ExitCode, await stdoutTask.ConfigureAwait(false), await stderrTask.ConfigureAwait(false));
    }

    private sealed record AgentContext(Dictionary<string, string> EnvironmentVariables) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            if (EnvironmentVariables.Count == 0)
            {
                return;
            }

            await RunProcessAsync("ssh-agent", EnvironmentVariables, "-k");
        }
    }

    private sealed class TempDirectory : IDisposable
    {
        public TempDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));
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
                // ignore cleanup failures
            }
        }
    }

    private sealed class EnvironmentScope : IDisposable
    {
        private readonly Dictionary<string, string?> originals = new(StringComparer.Ordinal);

        public EnvironmentScope(IDictionary<string, string> values)
        {
            foreach (var (key, value) in values)
            {
                originals[key] = Environment.GetEnvironmentVariable(key);
                Environment.SetEnvironmentVariable(key, value);
            }
        }

        public void Dispose()
        {
            foreach (var (key, value) in originals)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    private readonly record struct ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
