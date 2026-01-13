using System.Reflection;
using Common.EventBus.Module;
using Eleon.TestsBase.Lib.TestHelpers;
using Logging.Module;
using NSubstitute;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using VPortal.Auditor.Module.DomainServices;
using VPortal.Auditor.Module.Repositories;

namespace Eleon.Auditor.Module.Tests.TestHelpers;

internal static class AuditTestHelpers
{
  public static IDistributedEventBus CreateResponseCapableEventBus()
  {
    var bus = Substitute.For<IDistributedEventBus, IResponseCapableEventBus>();
    bus.PublishAsync(Arg.Any<object>(), Arg.Any<bool>(), Arg.Any<bool>()).Returns(Task.CompletedTask);
    return bus;
  }

  public static AuditDomainService CreateDomainService(
      IAuditHistoryRecordRepository? historyRecordRepository = null,
      IAuditDataRepository? dataRepository = null,
      IAuditCurrentVersionRepository? currentVersionRepository = null,
      ICurrentUser? currentUser = null,
      IDistributedEventBus? eventBus = null,
      IVportalLogger<AuditDomainService>? logger = null,
      IGuidGenerator? guidGenerator = null)
  {
    var service = new AuditDomainService(
        logger ?? TestMockHelpers.CreateMockLogger<AuditDomainService>(),
        historyRecordRepository ?? Substitute.For<IAuditHistoryRecordRepository>(),
        dataRepository ?? Substitute.For<IAuditDataRepository>(),
        currentVersionRepository ?? Substitute.For<IAuditCurrentVersionRepository>(),
        currentUser ?? TestMockHelpers.CreateMockCurrentUser(),
        eventBus ?? CreateResponseCapableEventBus());

    var generator = guidGenerator ?? TestMockHelpers.CreateMockGuidGenerator();
    var lazyServiceProvider = Substitute.For<IAbpLazyServiceProvider>();
    lazyServiceProvider.LazyGetRequiredService<IGuidGenerator>().Returns(generator);
    lazyServiceProvider.LazyGetRequiredService(typeof(IGuidGenerator)).Returns(generator);
    lazyServiceProvider.LazyGetService<IGuidGenerator>().Returns(generator);
    lazyServiceProvider.LazyGetService(typeof(IGuidGenerator)).Returns(generator);

    var lazyServiceProviderProp = typeof(DomainService)
        .GetProperty("LazyServiceProvider", BindingFlags.Public | BindingFlags.Instance);
    if (lazyServiceProviderProp is { CanWrite: true })
    {
      lazyServiceProviderProp.SetValue(service, lazyServiceProvider);
    }

    var guidGeneratorProp = typeof(DomainService)
        .GetProperty("GuidGenerator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    if (guidGeneratorProp is { CanWrite: true })
    {
      guidGeneratorProp.SetValue(service, generator);
    }
    else
    {
      var guidGeneratorField = typeof(DomainService)
          .GetField("_guidGenerator", BindingFlags.NonPublic | BindingFlags.Instance)
          ?? typeof(DomainService).GetField("guidGenerator", BindingFlags.NonPublic | BindingFlags.Instance);
      guidGeneratorField?.SetValue(service, generator);
    }

    return service;
  }

  public static ICurrentUser CreateCurrentUser(
      Guid? id,
      string? name,
      string? surName,
      string? userName)
  {
    var currentUser = Substitute.For<ICurrentUser>();
    currentUser.Id.Returns(id);
    currentUser.Name.Returns(name);
    currentUser.SurName.Returns(surName);
    currentUser.UserName.Returns(userName);
    return currentUser;
  }
}
