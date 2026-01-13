using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.TenantManagement.Module.Autodetect
{
  public interface IServersideAutodetectAppService : IApplicationService
  {
    public Task<List<ApplicationModuleDto>> GetDetectedModules();
    public Task StartDetect();
  }
}
