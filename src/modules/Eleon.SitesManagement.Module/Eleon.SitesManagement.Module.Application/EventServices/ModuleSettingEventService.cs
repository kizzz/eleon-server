using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Managers;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.EventServices
{
  public class ModuleSettingEventService : IDistributedEventHandler<GetModuleSettingMsg>, ITransientDependency
  {
    private readonly IDistributedEventBus distributedEventBus;
    private readonly IVportalLogger<ModuleSettingEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly IUnitOfWorkManager unitOfWorkManager;
    private readonly IObjectMapper objectMapper;
    private readonly ICurrentTenant currentTenant;
    private readonly ModuleSettingsManager moduleSettingsManager;

    public ModuleSettingEventService(
        IDistributedEventBus distributedEventBus,
        IVportalLogger<ModuleSettingEventService> logger,
        IResponseContext responseContext,
        IObjectMapper objectMapper,
        ICurrentTenant currentTenant,
        ModuleSettingsManager moduleSettingsManager,
        IUnitOfWorkManager unitOfWorkManager
        )
    {
      this.distributedEventBus = distributedEventBus;
      this.logger = logger;
      this.responseContext = responseContext;
      this.objectMapper = objectMapper;
      this.currentTenant = currentTenant;
      this.moduleSettingsManager = moduleSettingsManager;
      this.unitOfWorkManager = unitOfWorkManager;
    }
    public async Task HandleEventAsync(GetModuleSettingMsg eventData)
    {

      ModuleSettingsGotMsg result = new ModuleSettingsGotMsg();
      try
      {
        using var uow = unitOfWorkManager.Begin();
        using (currentTenant.Change(eventData.TenantId))
        {
          result = await moduleSettingsManager.GetAsync();
        }
        await uow.SaveChangesAsync();
        await uow.CompleteAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
        await responseContext.RespondAsync(result);
      }
    }
  }
}


