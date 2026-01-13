using Logging.Module;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Microservices;
using VPortal.TenantManagement.Module.Autodetect;

namespace VPortal.SitesManagement.Module.Autodetect
{
  public class ServersideAutodetectAppService : SitesManagementAppService, IServersideAutodetectAppService
  {
    private readonly ClientApplicationDomainService _clientApplicationManager;
    private readonly IVportalLogger<ServersideAutodetectAppService> logger;
    private readonly ServersideAutodetectDomainService serversideAutodetectDomainService;

    public ServersideAutodetectAppService(ClientApplicationDomainService clientApplicationManager,
        IVportalLogger<ServersideAutodetectAppService> logger,
        ServersideAutodetectDomainService serversideAutodetectDomainService)
    {
      _clientApplicationManager = clientApplicationManager;
      this.logger = logger;
      this.serversideAutodetectDomainService = serversideAutodetectDomainService;
    }

    public async Task<List<ApplicationModuleDto>> GetDetectedModules()
    {
      List<ApplicationModuleDto> result = null;
      try
      {
        var entities = await serversideAutodetectDomainService.GetDetectedModules();
        result = ObjectMapper.Map<List<ModuleEntity>, List<ApplicationModuleDto>>(entities);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task StartDetect()
    {
      try
      {
        await serversideAutodetectDomainService.StartDetect();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }
  }
}


