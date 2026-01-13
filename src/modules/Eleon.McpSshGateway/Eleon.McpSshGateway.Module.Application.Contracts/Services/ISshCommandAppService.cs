using Eleon.McpSshGateway.Module.Application.Contracts.Dtos;
using Eleon.McpSshGateway.Module.Application.Contracts.Exceptions;
using Volo.Abp.Application.Services;

namespace Eleon.McpSshGateway.Module.Application.Contracts.Services;

public interface ISshCommandAppService : IApplicationService
{
    Task<ExecuteCommandResult> ExecuteAsync(ExecuteCommandInput input, CancellationToken cancellationToken);
}

