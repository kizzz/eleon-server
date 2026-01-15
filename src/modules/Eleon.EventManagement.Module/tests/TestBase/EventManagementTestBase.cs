using Eleon.Common.Lib.UserToken;
using Eleon.TestsBase.Lib.TestBase;
using EventManagementModule.Domain.EventServices;
using EventManagementModule.Module.Domain.Shared.Repositories;
using EventManagementModule.Module.Domain.Shared.Queues;
using Logging.Module;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using NSubstitute;
using TenantSettings.Module.Cache;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using SharedModule.modules.MultiTenancy.Module;
using VPortal.EventManagementModule.Module.Localization;

namespace VPortal.EventManagementModule.Module.Tests.TestBase;

public abstract class EventManagementTestBase : MockingTestBase
{
  protected IQueueRepository CreateQueueRepository()
  {
    return Substitute.For<IQueueRepository>();
  }

  protected IQueueDefinitionRepository CreateQueueDefinitionRepository()
  {
    return Substitute.For<IQueueDefinitionRepository>();
  }

  protected IQueueEngine CreateQueueEngine()
  {
    return Substitute.For<IQueueEngine>();
  }

  protected QueueDomainService CreateQueueDomainService(
      IQueueRepository queueRepository = null,
      IStringLocalizer<EventManagementModuleResource> localizer = null)
  {
    return new QueueDomainService(
        CreateMockLogger<QueueDomainService>(),
        queueRepository ?? CreateQueueRepository(),
        localizer ?? CreateLocalizer<EventManagementModuleResource>("key", "value"));
  }

  protected QueueDefinitionDomainService CreateQueueDefinitionDomainService(
      IQueueRepository queueRepository = null,
      IQueueDefinitionRepository queueDefinitionRepository = null,
      IDistributedEventBus eventBus = null,
      TenantCacheService tenantCacheService = null,
      IStringLocalizer<EventManagementModuleResource> localizer = null)
  {
    return new QueueDefinitionDomainService(
        CreateMockLogger<QueueDefinitionDomainService>(),
        localizer ?? CreateLocalizer<EventManagementModuleResource>("key", "value"),
        queueRepository ?? CreateQueueRepository(),
        queueDefinitionRepository ?? CreateQueueDefinitionRepository(),
        eventBus ?? CreateMockEventBus(),
        tenantCacheService ?? CreateTenantCacheService());
  }

  protected EventDomainService CreateEventDomainService(
      IQueueRepository queueRepository = null,
      IQueueDefinitionRepository queueDefinitionRepository = null,
      QueueDomainService queueDomainService = null,
      QueueDefinitionDomainService queueDefinitionDomainService = null,
      IQueueEngine queueEngine = null,
      QueueEngineOptions options = null,
      ICurrentPrincipalAccessor currentPrincipalAccessor = null,
      IUserTokenProvider userTokenProvider = null,
      IStringLocalizer<EventManagementModuleResource> localizer = null)
  {
    var repository = queueRepository ?? CreateQueueRepository();
    var localizerInstance = localizer ?? CreateLocalizer<EventManagementModuleResource>("key", "value");

    return new EventDomainService(
        CreateMockLogger<EventDomainService>(),
        queueDefinitionRepository ?? CreateQueueDefinitionRepository(),
        repository,
        queueDomainService ?? CreateQueueDomainService(repository, localizerInstance),
        queueDefinitionDomainService ?? CreateQueueDefinitionDomainService(
            repository,
            queueDefinitionRepository ?? CreateQueueDefinitionRepository(),
            CreateMockEventBus(),
            CreateTenantCacheService(),
            localizerInstance),
        localizerInstance,
        currentPrincipalAccessor ?? Substitute.For<ICurrentPrincipalAccessor>(),
        userTokenProvider ?? Substitute.For<IUserTokenProvider>(),
        queueEngine ?? CreateQueueEngine(),
        Options.Create(options ?? new QueueEngineOptions()));
  }

  protected TenantCacheService CreateTenantCacheService()
  {
    var currentTenant = Substitute.For<ICurrentTenant>();
    var options = Options.Create(new EleonMultiTenancyOptions { Enabled = false });
    return new TenantCacheService(CreateMockEventBus(), currentTenant, options);
  }
}
