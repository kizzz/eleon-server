using Common.Module.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Shared.Modules.Core.Module.Repositories
{
  public interface ISeriaNumbersRepository : IBasicRepository<SeriaNumberEntity, Guid>
  {
    Task<SeriaNumberEntity> GetSeriaNumberAsync(string documentObjectType, string refId, string prefix);
  }
}
