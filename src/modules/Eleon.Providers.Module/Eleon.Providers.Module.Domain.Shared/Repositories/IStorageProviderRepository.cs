using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Storage.Module.Entities;

namespace VPortal.Storage.Module.Repositories
{
  public interface IStorageProviderRepository : IBasicRepository<StorageProviderEntity, Guid>
  {
    Task<List<StorageProviderEntity>> GetStorageProvidersList(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null);
    Task<List<StorageProviderEntity>> GetStorageProvidersListByIds(List<Guid> ids);
  }
}
