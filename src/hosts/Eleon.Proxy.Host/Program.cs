using EleonsoftSdk.modules.Helpers.Module;

namespace Eleonsoft.ProxyAgent;

public class Program
{
  public async static Task<int> Main(string[] args) => await EleonsoftWebApplication.HostWebApplicationAsync<EleonProxyHostModule>(args);
}
