using Logging.Module;
using Messaging.Module.Messages;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Managers;

namespace VPortal.SitesManagement.Module.Microservices
{
  public class MicroserviceAppService : SitesManagementAppService, IMicroserviceAppService
  {
    private readonly IVportalLogger<MicroserviceAppService> logger;
    private readonly MicroserviceManager microserviceDomainService;
    private readonly ModuleSettingsManager moduleSettingsManager;

    public MicroserviceAppService(
        IVportalLogger<MicroserviceAppService> logger,
        MicroserviceManager microserviceDomainService,
        ModuleSettingsManager moduleSettingsManager)
    {
      this.logger = logger;
      this.microserviceDomainService = microserviceDomainService;
      this.moduleSettingsManager = moduleSettingsManager;
    }

    public async Task<EleoncoreModuleDto> Create(EleoncoreModuleDto input)
    {

      EleoncoreModuleDto result = null;
      try
      {
        var entity = await microserviceDomainService.CreateMicroservice(input.DisplayName, input.Path, input.IsEnabled, input.Source);
        result = ObjectMapper.Map<ModuleEntity, EleoncoreModuleDto>(entity);
        await moduleSettingsManager.RefreshAsync();
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

    public async Task<List<EleoncoreModuleDto>> GetMicroserviceList()
    {
      List<EleoncoreModuleDto> result = null;
      try
      {
        var entities = await microserviceDomainService.GetMicroservices();
        result = ObjectMapper.Map<List<ModuleEntity>, List<EleoncoreModuleDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> InitializeMicroservice(InitializeMicroserviceMsg initializeMicroserviceMsg)
    {
      bool result = false;
      try
      {
        await microserviceDomainService.InitializeMicroservice(initializeMicroserviceMsg.RequestId, initializeMicroserviceMsg.Info.ServiceId, initializeMicroserviceMsg.Info.DisplayName, initializeMicroserviceMsg.Info.Features);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}


