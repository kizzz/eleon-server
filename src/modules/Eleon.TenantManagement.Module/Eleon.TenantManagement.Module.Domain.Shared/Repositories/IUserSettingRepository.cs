using Volo.Abp.Domain.Repositories;
using VPortal.TenantManagement.Module.Entities;

namespace VPortal.TenantManagement.Module.Repositories
{
  public interface IUserSettingRepository : IBasicRepository<UserSettingEntity, Guid>
  {
    Task<UserSettingEntity> GetUserSettingByUserId(Guid userId);
    Task<List<UserSettingEntity>> GetUserSettingsByUserId(Guid userId);
  }
}
