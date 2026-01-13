using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Storage.Module.Entities;

namespace VPortal.Storage.Module.Repositories
{
  public interface IStorageProviderTypeRepository : IBasicRepository<StorageProviderTypeEntity, Guid>
  {
    Task<StorageProviderTypeEntity> GetByName(string name);
  }
}
