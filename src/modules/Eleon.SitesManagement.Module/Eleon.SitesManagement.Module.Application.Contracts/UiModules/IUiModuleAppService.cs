using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.UiModules
{
  public interface IUiModuleAppService
  {
    Task<EleoncoreModuleDto> GetAsync(Guid id);
    Task<List<EleoncoreModuleDto>> GetAllAsync();
    Task<EleoncoreModuleDto> CreateAsync(EleoncoreModuleDto input);
    Task<EleoncoreModuleDto> UpdateAsync(Guid id, EleoncoreModuleDto input);
    Task DeleteAsync(Guid id);
    Task<List<EleoncoreModuleDto>> GetModulesByApplicationAsync(Guid applicationId);
    Task<List<EleoncoreModuleDto>> GetEnabledModulesAsync();
  }
}


