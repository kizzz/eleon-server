using ServicesOrchestrator.Services.Abstractions;
using System.Net.Sockets;
using Volo.Abp.DependencyInjection;

namespace ServicesOrchestrator.Services.ServiceHandlers;

[ExposeServices(typeof(IServiceHandler))]
public sealed class ConnectionServiceHandler : IServiceHandler, ISingletonDependency
{
  public string Type => "connection";

  public async Task<bool> StatusAsync(ServiceConfig svc, CancellationToken ct)
  {
    if (string.IsNullOrWhiteSpace(svc.ConnectionString)) return false;

    // Try AMQP tcp first; else try HTTP HEAD if URL; else tcp by host:port in URI.
    if (Uri.TryCreate(svc.ConnectionString, UriKind.Absolute, out var u))
    {
      if (u.Scheme is "amqp" or "amqps" or "tcp")
      {
        var host = u.Host;
        var port = u.IsDefaultPort ? (u.Scheme == "amqps" ? 5671 : 5672) : u.Port;
        return await TcpPingAsync(host, port, ct);
      }
      if (u.Scheme.StartsWith("http", StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          using var http = new HttpClient();
          http.Timeout = TimeSpan.FromSeconds(3);
          using var req = new HttpRequestMessage(HttpMethod.Head, u);
          using var resp = await http.SendAsync(req, ct);
          return resp.IsSuccessStatusCode;
        }
        catch { return false; }
      }
    }

    return false;
  }

  public Task ChangeStatusAsync(ServiceConfig svc, ServiceCommand desired, CancellationToken ct)
      => Task.CompletedTask; // NO-OP

  private static async Task<bool> TcpPingAsync(string host, int port, CancellationToken ct)
  {
    try
    {
      using var client = new TcpClient();
      var connect = client.ConnectAsync(host, port);
      var delay = Task.Delay(TimeSpan.FromSeconds(2), ct);
      var done = await Task.WhenAny(connect, delay);
      return done == connect && client.Connected;
    }
    catch { return false; }
  }
}
