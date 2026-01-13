using System.Collections.Generic;
using System.Linq;
using Eleon.McpGateway.Module.Domain;

namespace Eleon.McpGateway.Module.Infrastructure.Backends;

public sealed class McpBackendRegistry : IMcpBackendRegistry
{
    private readonly IReadOnlyDictionary<string, IMcpBackend> backends;
    private readonly IMcpBackend defaultBackend;

    public McpBackendRegistry(IEnumerable<IMcpBackend> backends)
    {
        var dictionary = backends.ToDictionary(b => b.Name, StringComparer.OrdinalIgnoreCase);
        if (dictionary.Count == 0)
        {
            throw new InvalidOperationException("At least one MCP backend must be registered.");
        }

        this.backends = dictionary;
        defaultBackend = dictionary.Values.First();
    }

    public IMcpBackend GetBackend(string name)
    {
        if (!backends.TryGetValue(name, out var backend))
        {
            throw new KeyNotFoundException($"Backend '{name}' is not registered.");
        }

        return backend;
    }

    public IMcpBackend GetDefaultBackend() => defaultBackend;

    public IReadOnlyCollection<IMcpBackend> GetAll() => backends.Values.ToList();
}

