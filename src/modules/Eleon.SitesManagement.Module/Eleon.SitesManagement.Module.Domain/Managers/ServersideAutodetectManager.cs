using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;

namespace VPortal.SitesManagement.Module.Managers
{
  public class ServersideAutodetectManager : DomainService, ISingletonDependency
  {
    private readonly IVportalLogger<ServersideAutodetectDomainService> logger;
    private readonly IDistributedEventBus eventBus;

    private Guid? detectionRequestId = null;
    private object detectionLocker = new object();

    private List<ModuleEntity> detectedModules = null;

    public ServersideAutodetectManager(
        IVportalLogger<ServersideAutodetectDomainService> logger,
        IDistributedEventBus eventBus)
    {
      this.logger = logger;
      this.eventBus = eventBus;
    }

    public async Task<List<ModuleEntity>> GetDetectedModules()
    {
      List<ModuleEntity> result = new List<ModuleEntity>();
      try
      {
        result.AddRange(detectedModules);
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

    internal async Task StartDetect()
    {
      lock (detectionLocker)
      {
        detectionRequestId = GuidGenerator.Create();
        detectedModules = new List<ModuleEntity>();
      }

      var request = new TriggerMicroserviceInitializationMsg()
      {
        RequestId = detectionRequestId.Value,
      };

      await eventBus.PublishAsync(request);
    }

    internal async Task AddAutodetected(Guid requestId, ModuleEntity module)
    {
      lock (detectionLocker)
      {
        if (detectionRequestId != requestId)
        {
          return;
        }

        detectedModules.Add(module);
      }
    }
  }
}


