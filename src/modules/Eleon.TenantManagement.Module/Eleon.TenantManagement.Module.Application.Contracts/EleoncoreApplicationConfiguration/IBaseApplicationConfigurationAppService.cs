using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration
{
  public interface IBaseApplicationConfigurationAppService
  {
    Task<EleoncoreApplicationConfigurationDto> GetBaseAsync();
  }
}
