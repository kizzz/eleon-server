using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.TenantManagement.Module.Entities;

namespace VPortal.TenantManagement.Module.Repositories
{
  public interface ITenantSettingRepository : IBasicRepository<TenantSettingEntity, Guid>
  {
    Task<TenantSettingEntity> GetByTenantId(Guid? tenantId);
  }
}
