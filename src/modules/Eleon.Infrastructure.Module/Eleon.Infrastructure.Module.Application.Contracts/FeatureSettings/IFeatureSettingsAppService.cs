using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Core.Infrastructure.Module.FeatureSettings
{
  public interface IFeatureSettingsAppService : IApplicationService
  {
    Task<List<FeatureSettingDto>> SetAsync(List<SetFeatureSettingDto> settings);
    Task<FeatureSettingDto> GetAsync(string group, string key);
  }
}
