namespace Eleon.McpSshGateway.Module.Domain.ValueObjects;

public sealed record SshCredential
(
    SshCredentialType Type,
    string? Password,
    string? PrivateKeyPath,
    string? PrivateKeyPassphrase
)
{
    public static SshCredential PasswordCredential(string password) =>
        new(SshCredentialType.Password, password, null, null);

    public static SshCredential PrivateKeyCredential(string privateKeyPath, string? privateKeyPassphrase = null) =>
        new(SshCredentialType.PrivateKey, null, privateKeyPath, privateKeyPassphrase);

    public static SshCredential AgentCredential() => new(SshCredentialType.Agent, null, null, null);
}

