using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration
{
  public interface IEleoncoreApplicationConfigurationAppService
  {
    Task<EleoncoreApplicationConfigurationDto> Get();
    Task<EleoncoreApplicationConfigurationDto> GetByAppIdAsync(string appId);
  }
}


