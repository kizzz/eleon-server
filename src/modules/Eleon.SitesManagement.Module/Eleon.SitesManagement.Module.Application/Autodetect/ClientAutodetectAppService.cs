using Logging.Module;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.Autodetect
{
  public class ClientAutodetectAppService : SitesManagementAppService, IClientAutodetectAppService
  {
    private readonly ClientApplicationDomainService _clientApplicationManager;
    private readonly IVportalLogger<ClientAutodetectAppService> logger;
    private readonly ClientAutodetectDomainService clientAutodetectDomainService;

    public ClientAutodetectAppService(ClientApplicationDomainService clientApplicationManager,
        IVportalLogger<ClientAutodetectAppService> logger,
        ClientAutodetectDomainService clientAutodetectDomainService)
    {
      _clientApplicationManager = clientApplicationManager;
      this.logger = logger;
      this.clientAutodetectDomainService = clientAutodetectDomainService;
    }

    public async Task<List<ApplicationModuleDto>> GetDetectedProxy(Guid proxyId)
    {

      List<ApplicationModuleDto> result = null;
      try
      {
        string url = "GET FROM PROXY";
        var entities = await clientAutodetectDomainService.GetDetected(url);
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

    public async Task<List<ApplicationModuleDto>> GetDetectedWeb(string url)
    {

      List<ApplicationModuleDto> result = null;
      try
      {
        var entities = await clientAutodetectDomainService.GetDetected(url);
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
  }
}


