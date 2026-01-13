namespace Eleon.Ssh;

public interface ISshCommandRunner
{
    Task<SshCommandResult> RunAsync(SshConnectionInfo connection, string command, TimeSpan timeout, CancellationToken cancellationToken);
}
