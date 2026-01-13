using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Managers;

namespace VPortal.SitesManagement.Module.DomainServices
{
  public class ClientAutodetectDomainService : DomainService
  {
    private readonly IVportalLogger<ClientAutodetectDomainService> logger;
    private readonly ClientAutodetectManager clientAutodetectManager;

    public ClientAutodetectDomainService(
        IVportalLogger<ClientAutodetectDomainService> logger,
        ClientAutodetectManager clientAutodetectManager
        )
    {
      this.logger = logger;
      this.clientAutodetectManager = clientAutodetectManager;
    }

    public async Task<List<ModuleEntity>> GetDetected(string url)
    {
      List<ModuleEntity> result = null;
      try
      {
        result = await clientAutodetectManager.GetDetected(url);
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


