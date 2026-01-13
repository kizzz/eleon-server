using Eleon.McpSshGateway.Domain.Entities;
using Eleon.McpSshGateway.Domain.Services;
using Eleon.McpSshGateway.Domain.ValueObjects;
using FluentAssertions;

namespace Eleon.McpSshGateway.Domain.Tests;

public sealed class CommandPolicyServiceTests
{
    private static readonly SshCredential Credential = SshCredential.AgentCredential();

    private readonly CommandPolicyService service = new();

    [Fact]
    public void Denies_When_Host_Disabled()
    {
        var host = CreateHost(isEnabled: false, allow: new[] { "*" });

        service.IsAllowed(host, "ls").Should().BeFalse();
    }

    [Fact]
    public void Denies_When_Command_Not_In_Allow_List()
    {
        var host = CreateHost(allow: new[] { "ls *" });

        service.IsAllowed(host, "cat /etc/passwd").Should().BeFalse();
    }

    [Fact]
    public void Denies_When_Command_On_Deny_List()
    {
        var host = CreateHost(allow: new[] { "*" }, deny: new[] { "rm *" });

        service.IsAllowed(host, "rm -rf /tmp").Should().BeFalse();
    }

    [Fact]
    public void Allows_When_Command_Matches_Allow_List()
    {
        var host = CreateHost(allow: new[] { "ls *" });

        service.IsAllowed(host, "ls /var").Should().BeTrue();
    }

    private static SshHost CreateHost(bool isEnabled = true, string[]? allow = null, string[]? deny = null)
    {
        return new SshHost(
            "host",
            "Host",
            "example",
            22,
            "user",
            Credential,
            allowPatterns: allow,
            denyPatterns: deny,
            isEnabled: isEnabled);
    }
}
