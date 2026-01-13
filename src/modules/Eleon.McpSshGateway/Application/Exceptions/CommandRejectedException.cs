namespace Eleon.McpSshGateway.Application.Exceptions;

public sealed class CommandRejectedException : Exception
{
    public CommandRejectedException(string message)
        : base(message)
    {
    }
}
