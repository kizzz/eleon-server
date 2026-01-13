using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using Sentry;
using SharedModule.modules.Helpers.Module.EventHandlers;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using VPortal.Lifecycle.Feature.Module.DomainServices;

namespace VPortal.Lifecycle.Feature.Module.EventServices
{
  public class AddLifecycleEventService : ConcurrencyAwareEventHandler<LifecycleRequestedMsg, AddLifecycleEventService>
  {
    private readonly LifecycleManagerDomainService lifecycleManagerDomainService;
    private readonly ICurrentPrincipalAccessor currentPrincipalAccessor;
    private readonly ICurrentTenant currentTenant;
    private readonly IDistributedEventBus massTransitPublisher;

    public AddLifecycleEventService(
        IVportalLogger<AddLifecycleEventService> logger,
        LifecycleManagerDomainService lifecycleManagerDomainService,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        ICurrentTenant currentTenant,
        IDistributedEventBus massTransitPublisher)
        : base(logger)
    {
      this.lifecycleManagerDomainService = lifecycleManagerDomainService;
      this.currentPrincipalAccessor = currentPrincipalAccessor;
      this.currentTenant = currentTenant;
      this.massTransitPublisher = massTransitPublisher;
    }

    protected override async Task HandleEventInternalAsync(LifecycleRequestedMsg eventData)
    {
      var newPrincipal = new ClaimsPrincipal(
          new ClaimsIdentity([new Claim(AbpClaimTypes.UserId, eventData.InitiatorId.ToString())]));

      using (currentPrincipalAccessor.Change(newPrincipal))
      {
        await lifecycleManagerDomainService.StartNewLifecycle(
            eventData.TemplateId,
            eventData.DocumentObjectType,
            eventData.DocumentId,
            extras: null,
            eventData.IsSkipFilled,
            eventData.StartImmediately);
      }
    }
  }
}
