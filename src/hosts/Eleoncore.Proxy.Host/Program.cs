using EleonsoftSdk.modules.Helpers.Module;

namespace Eleonsoft.Host;

public class Program
{
  public async static Task<int> Main(string[] args) => await EleoncoreWebApplication.HostWebApplicationAsync<EleoncoreProxyHostModule>(args);
}
