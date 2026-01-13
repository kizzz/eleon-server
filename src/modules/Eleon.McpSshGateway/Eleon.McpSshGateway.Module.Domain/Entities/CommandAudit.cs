namespace Eleon.McpSshGateway.Module.Domain.Entities;

public sealed class CommandAudit
{
    private const int DefaultPreviewLimit = 2048;

    public CommandAudit(Guid id, string hostId, string command, int exitCode, DateTimeOffset timestamp, string stdoutPreview, string stderrPreview, TimeSpan duration)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        HostId = string.IsNullOrWhiteSpace(hostId) ? throw new ArgumentException("HostId is required", nameof(hostId)) : hostId;
        Command = command ?? string.Empty;
        ExitCode = exitCode;
        Timestamp = timestamp;
        StdoutPreview = stdoutPreview ?? string.Empty;
        StderrPreview = stderrPreview ?? string.Empty;
        Duration = duration;
    }

    public Guid Id { get; }

    public string HostId { get; }

    public string Command { get; }

    public int ExitCode { get; }

    public DateTimeOffset Timestamp { get; }

    public string StdoutPreview { get; }

    public string StderrPreview { get; }

    public TimeSpan Duration { get; }

    public static CommandAudit FromResult(
        string hostId,
        string command,
        int exitCode,
        string stdout,
        string stderr,
        TimeSpan duration,
        int previewLimit = DefaultPreviewLimit) =>
        new(Guid.NewGuid(), hostId, command, exitCode, DateTimeOffset.UtcNow, Truncate(stdout, previewLimit), Truncate(stderr, previewLimit), duration);

    private static string Truncate(string value, int limit)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= limit)
        {
            return value ?? string.Empty;
        }

        return value[..limit];
    }
}

