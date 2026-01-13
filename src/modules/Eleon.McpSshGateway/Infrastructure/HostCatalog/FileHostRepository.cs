using System.IO;
using System.Text.Json;
using Eleon.McpSshGateway.Domain.Entities;
using Eleon.McpSshGateway.Domain.Repositories;
using Eleon.McpSshGateway.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eleon.McpSshGateway.Infrastructure.HostCatalog;

public sealed class FileHostRepository : IHostRepository, IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ILogger<FileHostRepository> logger;
    private readonly FileHostRepositoryOptions options;
    private readonly SemaphoreSlim reloadGate = new(1, 1);
    private IReadOnlyList<SshHost> cachedHosts = Array.Empty<SshHost>();
    private DateTimeOffset lastWrite;
    private bool disposed;

    public FileHostRepository(IOptions<FileHostRepositoryOptions> options, ILogger<FileHostRepository> logger)
    {
        this.logger = logger;
        this.options = options.Value ?? new FileHostRepositoryOptions();
    }

    public async Task<IReadOnlyList<SshHost>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await EnsureCacheAsync(cancellationToken).ConfigureAwait(false);
        return cachedHosts;
    }

    public async Task<SshHost?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        await EnsureCacheAsync(cancellationToken).ConfigureAwait(false);
        return cachedHosts.FirstOrDefault(h => string.Equals(h.Id, id, StringComparison.OrdinalIgnoreCase));
    }

    private async Task EnsureCacheAsync(CancellationToken cancellationToken)
    {
        var info = new FileInfo(options.CatalogPath);
        if (!info.Exists)
        {
            if (cachedHosts.Count == 0)
            {
                logger.LogWarning("Host catalog not found at {CatalogPath}", options.CatalogPath);
            }

            cachedHosts = Array.Empty<SshHost>();
            lastWrite = DateTimeOffset.MinValue;
            return;
        }

        if (info.LastWriteTimeUtc <= lastWrite && cachedHosts.Count > 0)
        {
            return;
        }

        await reloadGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            info.Refresh();
            if (info.LastWriteTimeUtc <= lastWrite && cachedHosts.Count > 0)
            {
                return;
            }

            await using var stream = info.OpenRead();
            var document = await JsonSerializer.DeserializeAsync<HostCatalogDocument>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false)
                            ?? new HostCatalogDocument();
            cachedHosts = document.Hosts.Select(MapHost).ToArray();
            lastWrite = info.LastWriteTimeUtc;
            logger.LogInformation("Loaded {HostCount} SSH host definitions", cachedHosts.Count);
        }
        finally
        {
            reloadGate.Release();
        }
    }

    private SshHost MapHost(HostDefinitionModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Id))
        {
            throw new InvalidDataException("Host id is required in the catalog.");
        }

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            throw new InvalidDataException($"Host '{model.Id}' is missing a name");
        }

        if (string.IsNullOrWhiteSpace(model.Hostname))
        {
            throw new InvalidDataException($"Host '{model.Id}' is missing a hostname");
        }

        if (string.IsNullOrWhiteSpace(model.Username))
        {
            throw new InvalidDataException($"Host '{model.Id}' is missing a username");
        }

        var credential = CreateCredential(model);
        return new SshHost(
            model.Id,
            model.Name,
            model.Hostname,
            model.Port ?? 22,
            model.Username,
            credential,
            model.Tags,
            model.Allow,
            model.Deny,
            model.Enabled);
    }

    private SshCredential CreateCredential(HostDefinitionModel model)
    {
        var auth = model.Auth ?? new HostAuthModel { Mode = "agent" };
        var mode = auth.Mode ?? "agent";
        return mode.ToLowerInvariant() switch
        {
            "password" => SshCredential.PasswordCredential(ResolveSecret(auth.Password, auth.PasswordEnv) ?? throw new InvalidDataException($"Host '{model.Id}' is missing a password")),
            "privatekey" => SshCredential.PrivateKeyCredential(
                auth.PrivateKeyPath ?? throw new InvalidDataException($"Host '{model.Id}' is missing a privateKeyPath"),
                ResolveSecret(auth.PrivateKeyPassphrase, auth.PrivateKeyPassphraseEnv)),
            "agent" => SshCredential.AgentCredential(),
            _ => throw new InvalidDataException($"Host '{model.Id}' has unsupported auth mode '{auth.Mode}'")
        };
    }

    private static string? ResolveSecret(string? inline, string? envName)
    {
        if (!string.IsNullOrEmpty(inline))
        {
            return inline;
        }

        if (string.IsNullOrWhiteSpace(envName))
        {
            return null;
        }

        return Environment.GetEnvironmentVariable(envName);
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        reloadGate.Dispose();
        disposed = true;
    }

    private sealed class HostCatalogDocument
    {
        public List<HostDefinitionModel> Hosts { get; set; } = new();
    }

    private sealed class HostDefinitionModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? Hostname { get; set; }

        public int? Port { get; set; }

        public string? Username { get; set; }

        public bool Enabled { get; set; } = true;

        public string[]? Tags { get; set; }

        public string[]? Allow { get; set; }

        public string[]? Deny { get; set; }

        public HostAuthModel? Auth { get; set; }
    }

    private sealed class HostAuthModel
    {
        public string? Mode { get; set; }

        public string? Password { get; set; }

        public string? PasswordEnv { get; set; }

        public string? PrivateKeyPath { get; set; }

        public string? PrivateKeyPassphrase { get; set; }

        public string? PrivateKeyPassphraseEnv { get; set; }
    }
}
