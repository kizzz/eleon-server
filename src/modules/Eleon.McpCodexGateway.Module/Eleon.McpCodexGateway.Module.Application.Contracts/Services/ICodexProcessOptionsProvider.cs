using Eleon.McpCodexGateway.Module.Domain.ValueObjects;

namespace Eleon.McpCodexGateway.Module.Application.Contracts.Services;

public interface ICodexProcessOptionsProvider
{
    CodexProcessOptions GetOptions();
}

