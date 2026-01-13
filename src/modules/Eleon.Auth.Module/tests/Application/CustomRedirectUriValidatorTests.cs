using System;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.EventBus.Module;
using Commons.Module.Messages.RedirectUri;
using IdentityServer4.Models;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Identity.Module.Application.IdentityServerServices;
using Xunit;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Application;

public class CustomRedirectUriValidatorTests : DomainTestBase
{
    private CustomRedirectUriValidator BuildService(TestCurrentTenant currentTenant, IResponseCapableEventBus bus)
    {
        var logger = CreateMockLogger<CustomRedirectUriValidator>();
        var eventBus = (IDistributedEventBus)bus;

        return new CustomRedirectUriValidator(
            currentTenant,
            logger,
            eventBus);
    }

    [Fact]
    public async Task IsRedirectUriValidAsync_ValidUri_ReturnsTrue()
    {
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<ValidateRedirectUriResponseMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new ValidateRedirectUriResponseMsg { IsValid = true }));
        bus.RequestAsync<ValidateRedirectUriResponseMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ValidateRedirectUriResponseMsg { IsValid = true }));

        var service = BuildService(currentTenant, bus);
        var client = new Client { ClientId = "test-client" };

        var result = await service.IsRedirectUriValidAsync("https://example.com/signin-oidc", client);

        Assert.True(result);
    }

    [Fact]
    public async Task IsRedirectUriValidAsync_InvalidUri_ReturnsFalse()
    {
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<ValidateRedirectUriResponseMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new ValidateRedirectUriResponseMsg { IsValid = false }));
        bus.RequestAsync<ValidateRedirectUriResponseMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ValidateRedirectUriResponseMsg { IsValid = false }));

        var service = BuildService(currentTenant, bus);
        var client = new Client { ClientId = "test-client" };

        var result = await service.IsRedirectUriValidAsync("https://malicious.com/callback", client);

        Assert.False(result);
    }

    [Fact]
    public async Task IsPostLogoutRedirectUriValidAsync_ValidUri_ReturnsTrue()
    {
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<ValidateRedirectUriResponseMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new ValidateRedirectUriResponseMsg { IsValid = true }));
        bus.RequestAsync<ValidateRedirectUriResponseMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ValidateRedirectUriResponseMsg { IsValid = true }));

        var service = BuildService(currentTenant, bus);
        var client = new Client { ClientId = "test-client" };

        var result = await service.IsPostLogoutRedirectUriValidAsync("https://example.com/signin-oidc", client);

        Assert.True(result);
    }

    [Fact]
    public async Task IsPostLogoutRedirectUriValidAsync_InvalidUri_ReturnsFalse()
    {
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<ValidateRedirectUriResponseMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new ValidateRedirectUriResponseMsg { IsValid = false }));
        bus.RequestAsync<ValidateRedirectUriResponseMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ValidateRedirectUriResponseMsg { IsValid = false }));

        var service = BuildService(currentTenant, bus);
        var client = new Client { ClientId = "test-client" };

        var result = await service.IsPostLogoutRedirectUriValidAsync("https://malicious.com/logout", client);

        Assert.False(result);
    }
}

