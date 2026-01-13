using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.FileManager.Module.Entities;

namespace VPortal.FileManager.Module.Repositories
{
  public interface IArchiveRepository : IBasicRepository<FileArchiveEntity, Guid>
  {
    Task<KeyValuePair<long, List<FileArchiveEntity>>> GetListAsyncByParams(
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            string searchQuery = null);
  }
}
