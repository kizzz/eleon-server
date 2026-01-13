using SharedModule.modules.Blob.Module.Models;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Storage.Module.StorageProviders
{
  public interface IStorageProvidersTestAppService : IApplicationService
  {
    Task<bool> TestStorageProvider(StorageProviderDto provider);
  }
}
