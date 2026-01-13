using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.Autodetect
{
  public interface IClientAutodetectAppService : IApplicationService
  {
    public Task<List<ApplicationModuleDto>> GetDetectedWeb(string url);
    public Task<List<ApplicationModuleDto>> GetDetectedProxy(Guid proxyId);
  }
}


