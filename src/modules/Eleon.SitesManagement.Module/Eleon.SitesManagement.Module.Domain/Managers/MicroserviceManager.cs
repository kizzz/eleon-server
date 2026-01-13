using Common.Module.Constants;
using Common.Module.Permissions;

//using Common.Module.Permissions;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Managers;
using VPortal.SitesManagement.Module.Permissions;

namespace VPortal.SitesManagement.Module.Microservices
{

  public class MicroserviceManager : DomainService
  {
    private readonly IVportalLogger<MicroserviceManager> logger;
    private readonly ModuleDomainService modulesDomainService;
    private readonly ServersideAutodetectManager serversideAutodetectManager;
    //private readonly FeaturePermissionManager featurePermissionManager;

    public MicroserviceManager(
        IVportalLogger<MicroserviceManager> logger,
        ModuleDomainService modulesDomainService,
        ServersideAutodetectManager serversideAutodetectManager
        //,FeaturePermissionManager featurePermissionManager
        )
    {
      this.logger = logger;
      this.modulesDomainService = modulesDomainService;
      this.serversideAutodetectManager = serversideAutodetectManager;
      //this.featurePermissionManager = featurePermissionManager;
    }

    public async Task<ModuleEntity> CreateMicroservice(string displayName, string path, bool isEnabled, string source)
    {
      ModuleEntity result = null;
      try
      {
        result = await modulesDomainService.CreateAsync(displayName, path, isEnabled, source, ModuleType.Resource);
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

    public async Task InitializeMicroservice(Guid? requestId, Guid id, string displayName, List<FeatureGroupDescription> features)
    {
      try
      {
        var microservice = await modulesDomainService.FindAsync(id);
        if (requestId != null)
        {
          var detected = microservice ?? new ModuleEntity(id)
          {
            DisplayName = displayName,
            HealthCheckStatus = Common.Module.Constants.ServiceHealthStatus.Healthy,
          };

          await serversideAutodetectManager.AddAutodetected(requestId.Value, detected);
        }

        //if (microservice != null)
        //{
        //    await featurePermissionManager.EnsureServiceFeatures(id, features);
        //}
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<List<ModuleEntity>> GetMicroservices()
    {
      try
      {
        return await modulesDomainService.GetByAsync(m => m.Type == ModuleType.Server);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return null;
    }

    public async Task<(List<Guid> busIds, List<Guid> httpIds)> GetServiceIdsToHealthCheck()
    {
      List<Guid> busIds = null;
      List<Guid> httpIds = null;
      try
      {
        var microservices = await modulesDomainService.GetListAsync();
        var now = DateTime.UtcNow;
        //busIds = microservices
        //    .Where(x => x.HealthChecks.Any(h => h.IsEnabled))
        //    .Where(x => ShouldBeChecked(now, x.HealthChecks..HealthCheckPeriodMinutes, x.LastBusHealthCheck))
        //    .Select(x => x.Id)
        //    .ToList();

        //httpIds = microservices
        //    .Where(x => x.EnableHttpHealthCheck)
        //    .Where(x => ShouldBeChecked(now, x.HealthCheckPeriodMinutes, x.LastHttpHealthCheck))
        //    .Select(x => x.Id)
        //    .ToList();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return (busIds, httpIds);
    }

    public async Task SetBusHealthChecked(Guid serviceId)
    {
      try
      {
        var service = await modulesDomainService.FindAsync(serviceId);
        //service.LastBusHealthCheck = DateTime.UtcNow;
        //service.BusHealthStatus = Common.Module.Constants.ServiceHealthStatus.Healthy;
        // await modulesDomainService.UpdateAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private static bool ShouldBeChecked(DateTime now, int periodMinutes, DateTime? lastHealthCheck)
    {
      if (lastHealthCheck == null)
      {
        return true;
      }

      return lastHealthCheck.Value.AddMinutes(periodMinutes) <= now;
    }
  }
}


