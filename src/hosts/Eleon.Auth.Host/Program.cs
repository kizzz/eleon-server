using EleonsoftSdk.modules.Helpers.Module;

namespace VPortal;

public class Program
{
  public async static Task<int> Main(string[] args)
  {
    return await EleonsoftWebApplication.HostWebApplicationAsync<AuthHostModule>(args);
  }
}
