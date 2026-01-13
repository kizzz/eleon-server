using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Storage.Module.LightweightStorageItems
{
  public interface ILightweightStorageItemAppService : IApplicationService
  {
    Task<string> GetLightweightItem(string key);
    Task<List<string>> GetLightweightItems(List<string> keys);
  }
}
