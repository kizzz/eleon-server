using System;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.EventBus.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using VPortal.Identity.Module.DomainServices;
using VPortal.Identity.Module.Localization;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class PasswordRestoreDomainServiceTests : DomainTestBase
{
    private PasswordRestoreDomainService BuildService(
        TestIdentityUserManager userManager,
        IResponseCapableEventBus requestClient,
        IResponseCapableEventBus massTransitPublisher,
        IResponseCapableEventBus eventBus,
        DefaultHttpContext httpContext = null)
    {
        var logger = CreateMockLogger<PasswordRestoreDomainService>();
        var localizer = Substitute.For<IStringLocalizer<IdentityResource>>();
        localizer["RepeatPassword:LinkMessage", Arg.Any<object>()]
            .Returns(new LocalizedString("RepeatPassword:LinkMessage", "Reset password link: {0}"));

        httpContext ??= new DefaultHttpContext
        {
            Request =
            {
                Host = new HostString("example.com")
            }
        };
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };

        return new PasswordRestoreDomainService(
            logger,
            (IDistributedEventBus)requestClient,
            localizer,
            (IDistributedEventBus)massTransitPublisher,
            httpContextAccessor,
            (IDistributedEventBus)eventBus,
            userManager);
    }

    [Fact]
    public async Task SendRestoreRequest_UserNotFound_ReturnsEmptyString()
    {
        var userManager = new TestIdentityUserManager();
        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.SendRestoreRequest("nonexistent@example.com", "nonexistent");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SendRestoreRequest_EmailUsernameMismatch_ReturnsEmptyString()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.SendRestoreRequest("user@example.com", "differentuser");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SendRestoreRequest_ValidUser_SendsRestoreLink()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        requestClient.RequestAsync<ActionCompletedMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new ActionCompletedMsg { Success = true }));
        requestClient.RequestAsync<ActionCompletedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ActionCompletedMsg { Success = true }));

        requestClient.RequestAsync<SendExternalLinkCreatedMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new SendExternalLinkCreatedMsg
            {
                ExternalLinkCreated = new ExternalLinkEto { FullLink = "https://example.com/reset?code=abc123" }
            }));
        requestClient.RequestAsync<SendExternalLinkCreatedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new SendExternalLinkCreatedMsg
            {
                ExternalLinkCreated = new ExternalLinkEto { FullLink = "https://example.com/reset?code=abc123" }
            }));

        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var distributedPublisher = (IDistributedEventBus)massTransitPublisher;
        distributedPublisher
            .PublishAsync(Arg.Any<NotificatorRequestedBulkMsg>(), Arg.Any<bool>(), Arg.Any<bool>())
            .Returns(Task.CompletedTask);
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        eventBus.RequestAsync<UserOtpSettingsGotMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new UserOtpSettingsGotMsg { Settings = new UserOtpSettingsEto() }));
        eventBus.RequestAsync<UserOtpSettingsGotMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new UserOtpSettingsGotMsg { Settings = new UserOtpSettingsEto() }));

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.SendRestoreRequest("user@example.com", "user");

        Assert.Empty(result);
        await requestClient.Received().RequestAsync<ActionCompletedMsg>(Arg.Any<object>());
        await requestClient.Received().RequestAsync<SendExternalLinkCreatedMsg>(Arg.Any<object>(), Arg.Any<int>());
    }

    [Fact]
    public async Task SendRestoreRequest_RecentLinkExists_ThrowsUserFriendlyException()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        requestClient.RequestAsync<ActionCompletedMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new ActionCompletedMsg { Success = false }));
        requestClient.RequestAsync<ActionCompletedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ActionCompletedMsg { Success = false }));

        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.SendRestoreRequest("user@example.com", "user");

        Assert.Empty(result);
    }

    [Fact]
    public async Task ValidateRestoreCode_EmptyCode_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.ValidateRestoreCode(null);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateRestoreCode_WhitespaceCode_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.ValidateRestoreCode("   ");

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateRestoreCode_InvalidCode_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        requestClient.RequestAsync<SendExternalLinkPublicParamsMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new SendExternalLinkPublicParamsMsg { IsSuccess = false }));
        requestClient.RequestAsync<SendExternalLinkPublicParamsMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new SendExternalLinkPublicParamsMsg { IsSuccess = false }));

        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.ValidateRestoreCode("invalid-code");

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateRestoreCode_ValidCode_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        requestClient.RequestAsync<SendExternalLinkPublicParamsMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new SendExternalLinkPublicParamsMsg { IsSuccess = true }));
        requestClient.RequestAsync<SendExternalLinkPublicParamsMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new SendExternalLinkPublicParamsMsg { IsSuccess = true }));

        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.ValidateRestoreCode("valid-code");

        Assert.True(result);
    }

    [Fact]
    public async Task ChangePassword_InvalidCode_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new SendExternalLinkPrivateParamsMsg { IsSuccess = false }));
        requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new SendExternalLinkPrivateParamsMsg { IsSuccess = false }));

        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.ChangePassword("invalid-code", "NewPassword123!");

        Assert.False(result);
    }

    [Fact]
    public async Task ChangePassword_CodeWithEmptyPrivateParams_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new SendExternalLinkPrivateParamsMsg
            {
                IsSuccess = true,
                PrivateParams = string.Empty
            }));
        requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new SendExternalLinkPrivateParamsMsg
            {
                IsSuccess = true,
                PrivateParams = string.Empty
            }));

        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.ChangePassword("code", "NewPassword123!");

        Assert.False(result);
    }

    [Fact]
    public async Task ChangePassword_UserNotFound_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new SendExternalLinkPrivateParamsMsg
            {
                IsSuccess = true,
                PrivateParams = Guid.NewGuid().ToString()
            }));
        requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new SendExternalLinkPrivateParamsMsg
            {
                IsSuccess = true,
                PrivateParams = Guid.NewGuid().ToString()
            }));

        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.ChangePassword("code", "NewPassword123!");

        Assert.False(result);
    }

    [Fact]
    public async Task ChangePassword_ValidCode_ChangesPassword()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new SendExternalLinkPrivateParamsMsg
            {
                IsSuccess = true,
                PrivateParams = user.Id.ToString()
            }));
        requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new SendExternalLinkPrivateParamsMsg
            {
                IsSuccess = true,
                PrivateParams = user.Id.ToString()
            }));

        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.ChangePassword("valid-code", "NewPassword123!");

        Assert.True(result);
    }

    [Fact]
    public async Task ChangePassword_ValidCode_UpdatesSecurityStamp()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new SendExternalLinkPrivateParamsMsg
            {
                IsSuccess = true,
                PrivateParams = user.Id.ToString()
            }));
        requestClient.RequestAsync<SendExternalLinkPrivateParamsMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new SendExternalLinkPrivateParamsMsg
            {
                IsSuccess = true,
                PrivateParams = user.Id.ToString()
            }));

        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.ChangePassword("valid-code", "NewPassword123!");

        Assert.True(result);
    }

    [Fact]
    public async Task SendRestoreRequest_ExceptionThrown_ReturnsErrorMessage()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var requestClient = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        requestClient.RequestAsync<ActionCompletedMsg>(Arg.Any<object>())
            .Returns(Task.FromException<ActionCompletedMsg>(new Exception("Event bus error")));
        requestClient.RequestAsync<ActionCompletedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromException<ActionCompletedMsg>(new Exception("Event bus error")));

        var massTransitPublisher = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var eventBus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, requestClient, massTransitPublisher, eventBus);
        var result = await service.SendRestoreRequest("user@example.com", "user");

        Assert.Empty(result);
    }
}
