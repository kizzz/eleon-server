using Eleon.McpGateway.Module.Domain;

namespace Eleon.McpGateway.Module.Infrastructure.Sessions;

public sealed class McpBackendFactoryRegistry
{
    private readonly Dictionary<string, IMcpBackendFactory> factories;

    public McpBackendFactoryRegistry(IEnumerable<IMcpBackendFactory> factories)
    {
        var factoryList = factories.ToList();
        this.factories = new Dictionary<string, IMcpBackendFactory>(StringComparer.OrdinalIgnoreCase);

        foreach (var factory in factoryList)
        {
            var backendName = GetBackendName(factory);
            if (this.factories.ContainsKey(backendName))
            {
                throw new InvalidOperationException($"Multiple factories registered for backend '{backendName}'.");
            }

            this.factories[backendName] = factory;
        }

        if (this.factories.Count == 0)
        {
            throw new InvalidOperationException("At least one MCP backend factory must be registered.");
        }
    }

    public IMcpBackendFactory GetFactory(string backendName)
    {
        if (!factories.TryGetValue(backendName, out var factory))
        {
            throw new KeyNotFoundException($"Backend factory for '{backendName}' is not registered.");
        }

        return factory;
    }

    private static string GetBackendName(IMcpBackendFactory factory)
    {
        return factory switch
        {
            CodexBackendFactory => "codex-stdio",
            SshBackendFactory => "ssh-stdio",
            _ => GetBackendNameFromProperty(factory) ?? throw new InvalidOperationException($"Unknown factory type: {factory.GetType().Name}")
        };
    }

    private static string? GetBackendNameFromProperty(IMcpBackendFactory factory)
    {
        // Support test factories that expose a BackendName property
        var property = factory.GetType().GetProperty("BackendName", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (property != null && property.GetValue(factory) is string backendName && !string.IsNullOrWhiteSpace(backendName))
        {
            return backendName;
        }
        return null;
    }
}

