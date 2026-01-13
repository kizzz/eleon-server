namespace Eleon.Ssh;

public sealed record SshConnectionInfo
{
    public string Host { get; init; } = string.Empty;

    public int Port { get; init; } = 22;

    public string Username { get; init; } = string.Empty;

    public SshAuthenticationMode AuthenticationMode { get; init; } = SshAuthenticationMode.Password;

    public string? Password { get; init; }

    public string? PrivateKeyPath { get; init; }

    public string? PrivateKeyPassphrase { get; init; }

    public TimeSpan KeepAliveInterval { get; init; } = TimeSpan.FromSeconds(30);

    public string? AgentExecutable { get; init; }
}
