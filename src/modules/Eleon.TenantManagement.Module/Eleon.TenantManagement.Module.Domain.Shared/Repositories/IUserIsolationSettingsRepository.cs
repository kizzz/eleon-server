using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.TenantManagement.Module.Entities;

namespace VPortal.TenantManagement.Module.Repositories
{
  public interface IUserIsolationSettingsRepository : IBasicRepository<UserIsolationSettingsEntity, Guid>
  {
    Task<UserIsolationSettingsEntity> GetByUserIdAsync(Guid userId);
  }
}
