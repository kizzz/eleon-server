using Core.Infrastructure.Module.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Core.Infrastructure.Module.Repositories
{
  public interface IDashboardSettingRepository : IBasicRepository<DashboardSettingEntity, Guid>
  {
    Task<List<DashboardSettingEntity>> GetDefaultDashboardSettings();
    Task<List<DashboardSettingEntity>> GetList(Guid userId);
  }
}
