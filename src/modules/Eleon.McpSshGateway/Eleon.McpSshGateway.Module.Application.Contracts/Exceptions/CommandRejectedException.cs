namespace Eleon.McpSshGateway.Module.Application.Contracts.Exceptions;

public sealed class CommandRejectedException : Exception
{
    public CommandRejectedException(string message)
        : base(message)
    {
    }
}

