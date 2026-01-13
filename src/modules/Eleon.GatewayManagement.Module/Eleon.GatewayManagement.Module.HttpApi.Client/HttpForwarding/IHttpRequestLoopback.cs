using System.Net.Http;
using System.Threading.Tasks;

namespace GatewayManagement.Module.HttpForwarding
{
  public interface IHttpRequestLoopback
  {
    Task<HttpResponseMessage> SendRequest(HttpRequestMessage requestMessage);
  }
}
