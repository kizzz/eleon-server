using Eleon.McpSshGateway.Domain.ValueObjects;

namespace Eleon.McpSshGateway.Domain.Entities;

public sealed class SshHost
{
    private static readonly string[] EmptyArray = Array.Empty<string>();

    public SshHost(
        string id,
        string name,
        string hostName,
        int port,
        string username,
        SshCredential credential,
        IEnumerable<string>? tags = null,
        IEnumerable<string>? allowPatterns = null,
        IEnumerable<string>? denyPatterns = null,
        bool isEnabled = true)
    {
        Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("Host id is required", nameof(id)) : id;
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Host name is required", nameof(name)) : name;
        HostName = string.IsNullOrWhiteSpace(hostName) ? throw new ArgumentException("HostName is required", nameof(hostName)) : hostName;
        Port = port <= 0 ? 22 : port;
        Username = string.IsNullOrWhiteSpace(username) ? throw new ArgumentException("Username is required", nameof(username)) : username;
        Credential = credential ?? throw new ArgumentNullException(nameof(credential));
        Tags = (tags ?? EmptyArray).Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        AllowPatterns = (allowPatterns ?? EmptyArray).Select(NormalizePattern).Where(p => p.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        DenyPatterns = (denyPatterns ?? EmptyArray).Select(NormalizePattern).Where(p => p.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        IsEnabled = isEnabled;
    }

    public string Id { get; }

    public string Name { get; }

    public string HostName { get; }

    public int Port { get; }

    public string Username { get; }

    public SshCredential Credential { get; }

    public IReadOnlyCollection<string> Tags { get; }

    public IReadOnlyCollection<string> AllowPatterns { get; }

    public IReadOnlyCollection<string> DenyPatterns { get; }

    public bool IsEnabled { get; private set; }

    public void Disable() => IsEnabled = false;

    public void Enable() => IsEnabled = true;

    private static string NormalizePattern(string pattern) => pattern?.Trim() ?? string.Empty;
}
