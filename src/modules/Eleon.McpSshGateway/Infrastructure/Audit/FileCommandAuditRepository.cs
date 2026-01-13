using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Eleon.McpSshGateway.Domain.Entities;
using Eleon.McpSshGateway.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eleon.McpSshGateway.Infrastructure.Audit;

public sealed class FileCommandAuditRepository : ICommandAuditRepository, IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    private readonly FileCommandAuditRepositoryOptions options;
    private readonly ILogger<FileCommandAuditRepository> logger;
    private readonly SemaphoreSlim writeGate = new(1, 1);
    private bool disposed;

    public FileCommandAuditRepository(IOptions<FileCommandAuditRepositoryOptions> options, ILogger<FileCommandAuditRepository> logger)
    {
        this.options = options.Value ?? new FileCommandAuditRepositoryOptions();
        this.logger = logger;
    }

    public async Task AddAsync(CommandAudit audit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(audit);

        await writeGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var directory = Path.GetDirectoryName(options.AuditLogPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var stream = new FileStream(options.AuditLogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            await using var writer = new StreamWriter(stream);
            var record = new AuditRecord
            {
                Id = audit.Id,
                HostId = audit.HostId,
                Command = audit.Command,
                ExitCode = audit.ExitCode,
                Timestamp = audit.Timestamp,
                DurationMilliseconds = audit.Duration.TotalMilliseconds,
                Stdout = audit.StdoutPreview,
                Stderr = audit.StderrPreview
            };
            var payload = JsonSerializer.Serialize(record, SerializerOptions);
            await writer.WriteLineAsync(payload).ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to write MCP SSH command audit entry");
            throw;
        }
        finally
        {
            writeGate.Release();
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        writeGate.Dispose();
        disposed = true;
    }

    private sealed class AuditRecord
    {
        public Guid Id { get; set; }

        public string HostId { get; set; } = string.Empty;

        public string Command { get; set; } = string.Empty;

        public int ExitCode { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public double DurationMilliseconds { get; set; }

        public string Stdout { get; set; } = string.Empty;

        public string Stderr { get; set; } = string.Empty;
    }
}
