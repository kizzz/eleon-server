using ProxyRouter.Minimal.HttpApi.Models.Messages;

namespace ProxyRouter.Minimal.HttpApi.Services;

public interface ILocationProvider
{
  public void Clear();
  public Task<List<Location>> GetAsync();
  Task<ModuleSettingsDto> GetModuleProperties(string baseUrl);
}
