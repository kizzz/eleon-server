using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace __NS__.__MOD_NAME__.HttpApi.Health;

internal static class TcpProbe
{
    public static async Task<bool> TryConnectAsync(string host, int port, TimeSpan timeout, CancellationToken ct)
    {
        using var client = new TcpClient();
        var connectTask = client.ConnectAsync(host, port);
        var delayTask = Task.Delay(timeout, ct);
        var completed = await Task.WhenAny(connectTask, delayTask);
        return completed == connectTask && client.Connected;
    }
}
