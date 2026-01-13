using System;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.EventBus.Module;
using Common.Module.Constants;
using Migrations.Module;
using Messaging.Module.Messages;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using VPortal.Identity.Module.DomainServices;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class ImpersonationDomainServiceTests : DomainTestBase
{
    private ImpersonationDomainService BuildService(
        TestIdentityUserManager userManager,
        IResponseCapableEventBus bus)
    {
        var logger = CreateMockLogger<ImpersonationDomainService>();
        var eventBus = (IDistributedEventBus)bus;

        var service = new ImpersonationDomainService(
            logger,
            eventBus,
            userManager);
        SetCurrentTenant(service, new TestCurrentTenant());
        return service;
    }

    private static void SetCurrentTenant(ImpersonationDomainService service, ICurrentTenant currentTenant)
    {
        var lazyServiceProvider = Substitute.For<IAbpLazyServiceProvider>();
        lazyServiceProvider.LazyGetRequiredService<ICurrentTenant>()
            .Returns(currentTenant);

        var prop = typeof(Volo.Abp.Domain.Services.DomainService)
            .GetProperty("LazyServiceProvider", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        prop?.SetValue(service, lazyServiceProvider);
    }

    [Fact]
    public async Task CheckIfCanImpersonate_Unauthorized_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<ControlDelegationCheckedMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new ControlDelegationCheckedMsg { IsDelegated = false }));
        bus.RequestAsync<ControlDelegationCheckedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ControlDelegationCheckedMsg { IsDelegated = false }));

        var service = BuildService(userManager, bus);
        var result = await service.CheckIfCanImpersonate(
            Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task CheckIfCanImpersonate_NotAdmin_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var actorUser = new IdentityUser(Guid.NewGuid(), "actor", "actor@example.com");
        var targetUser = new IdentityUser(Guid.NewGuid(), "target", "target@example.com");
        userManager.AddUser(actorUser);
        userManager.AddUser(targetUser);

        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<ControlDelegationCheckedMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new ControlDelegationCheckedMsg { IsDelegated = false }));
        bus.RequestAsync<ControlDelegationCheckedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ControlDelegationCheckedMsg { IsDelegated = false }));

        var service = BuildService(userManager, bus);
        var result = await service.CheckIfCanImpersonate(
            null, actorUser.Id,
            null, actorUser.Id,
            Guid.NewGuid(), targetUser.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckIfCanImpersonate_ValidAdmin_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var actorUser = new IdentityUser(Guid.NewGuid(), "admin", "admin@example.com");
        var targetUser = new IdentityUser(Guid.NewGuid(), "target", "target@example.com");
        userManager.AddUser(actorUser);
        userManager.AddUser(targetUser);
        await userManager.AddToRoleAsync(actorUser, MigrationConsts.AdminRoleNameDefaultValue);

        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<ControlDelegationCheckedMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new ControlDelegationCheckedMsg { IsDelegated = false }));
        bus.RequestAsync<ControlDelegationCheckedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ControlDelegationCheckedMsg { IsDelegated = false }));

        var service = BuildService(userManager, bus);
        var result = await service.CheckIfCanImpersonate(
            null, actorUser.Id,
            null, actorUser.Id,
            null, targetUser.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckIfCanImpersonate_SameUser_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var service = BuildService(userManager, bus);
        var result = await service.CheckIfCanImpersonate(
            null, user.Id,
            null, user.Id,
            null, user.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckIfCanImpersonate_ControlDelegated_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var actorUser = new IdentityUser(Guid.NewGuid(), "actor", "actor@example.com");
        var targetUser = new IdentityUser(Guid.NewGuid(), "target", "target@example.com");
        userManager.AddUser(actorUser);
        userManager.AddUser(targetUser);

        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<ControlDelegationCheckedMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new ControlDelegationCheckedMsg { IsDelegated = true }));
        bus.RequestAsync<ControlDelegationCheckedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ControlDelegationCheckedMsg { IsDelegated = true }));

        var service = BuildService(userManager, bus);
        var result = await service.CheckIfCanImpersonate(
            null, actorUser.Id,
            null, actorUser.Id,
            null, targetUser.Id);

        Assert.True(result);
    }
}
