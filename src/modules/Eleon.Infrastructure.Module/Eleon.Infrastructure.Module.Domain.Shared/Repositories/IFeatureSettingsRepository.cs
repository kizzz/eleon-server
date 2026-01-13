using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Core.Infrastructure.Module.Entities;

namespace VPortal.Core.Infrastructure.Module.Repositories
{
  public interface IFeatureSettingsRepository : IBasicRepository<FeatureSettingEntity, Guid>
  {
    Task<List<FeatureSettingEntity>> GetListByGroupAsync(Guid? tenantId, string group, int skipCount, int maxResultCount, string sorting);
    Task<long> GetCountListByGroupAsync(Guid? tenantId, string group);
    Task<FeatureSettingEntity> GetByKeyAsync(Guid? tenantId, string group, string key);

  }
}
