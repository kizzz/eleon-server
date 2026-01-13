using System;
using Xunit;

namespace Eleon.McpSshGateway.Module.Test.Integration;

internal static class SshIntegrationTestSettings
{
    internal const string EnableEnvVar = "ELEON_SSH_INTEGRATION";

    internal static void EnsureEnabled(string scenario)
    {
        var value = Environment.GetEnvironmentVariable(EnableEnvVar);
        if (!string.Equals(value, "1", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
        {
            Skip.If(true, $"{scenario} integration tests disabled. Set {EnableEnvVar}=1 to enable.");
        }
    }
}
