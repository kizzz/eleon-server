using Volo.Abp.Application.Services;

namespace VPortal.Storage.Remote.Application.Contracts.Storage
{
  public interface IStorageRemoteAppService : IApplicationService
  {
    Task<bool> Save(SaveBase64Request request);
    Task<string> GetBase64(string settingsGroup, string blobName);
    Task<bool> Delete(string settingsGroup, string blobName);
    Task<bool> Exists(string settingsGroup, string blobName);
  }
}
