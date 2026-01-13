using EleonsoftSdk.modules.Helpers.Module;

namespace Eleonsoft.Host;

public class Program
{
  public async static Task<int> Main(string[] args)
  {
    return await EleonsoftWebApplication.HostWebApplicationAsync<MinimalHostModule>(args);
  }
}
