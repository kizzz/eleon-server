namespace Eleon.Ssh;

public sealed class SshCommandException : Exception
{
    public SshCommandException(string message)
        : base(message)
    {
    }

    public SshCommandException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
