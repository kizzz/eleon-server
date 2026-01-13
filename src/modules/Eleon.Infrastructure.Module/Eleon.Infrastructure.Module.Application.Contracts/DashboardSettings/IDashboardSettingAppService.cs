using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Infrastructure.Module.DashboardSettings
{
  public interface IDashboardSettingAppService
  {
    Task<List<DashboardSettingDto>> GetDashboardSettings();
    Task<string> UpdateSettings(List<DashboardSettingDto> dashboardSettingDtos, bool setAsDefault);
    Task<string> DeleteDashboardSettings(Guid dashboardSettingEntityId);
  }
}
