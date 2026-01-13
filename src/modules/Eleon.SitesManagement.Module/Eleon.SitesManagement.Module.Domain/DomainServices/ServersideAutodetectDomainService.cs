using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Managers;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.DomainServices
{
  public class ServersideAutodetectDomainService : DomainService
  {
    private readonly IVportalLogger<ServersideAutodetectDomainService> logger;
    private readonly ServersideAutodetectManager serversideAutodetectManager;
    private readonly MicroserviceManager microserviceDomainService;

    public ServersideAutodetectDomainService(
        IVportalLogger<ServersideAutodetectDomainService> logger,
        ServersideAutodetectManager serversideAutodetectManager,
        MicroserviceManager microserviceDomainService
        )
    {
      this.logger = logger;
      this.serversideAutodetectManager = serversideAutodetectManager;
      this.microserviceDomainService = microserviceDomainService;
    }

    public async Task<List<ModuleEntity>> GetDetectedModules()
    {
      List<ModuleEntity> result = new List<ModuleEntity>();
      try
      {
        result = await serversideAutodetectManager.GetDetectedModules();

        var existingModules = await microserviceDomainService.GetMicroservices();

        result = result.Select(t =>
        {
          if (existingModules.Any(m => t.Path == m.Path))
          {
            //t.IsNew = true;
            t.HealthCheckStatusMessage = "Module is already added.";
          }

          return t;
        })
            .ToList();
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
        await serversideAutodetectManager.StartDetect();
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


