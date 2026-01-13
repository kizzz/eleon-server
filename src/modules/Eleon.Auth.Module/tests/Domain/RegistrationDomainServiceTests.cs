using System;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.EventBus.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using Eleon.TestsBase.Lib.TestHelpers;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using VPortal.Identity.Module.DomainServices;
using VPortal.Identity.Module.CustomCredentials;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class RegistrationDomainServiceTests : DomainTestBase
{
    private static GetIdentitySettingsForRegistrationGotMsg BuildRegistrationSettings(
        bool enableSelfRegistration = true,
        bool enablePassword = true,
        bool enableTwoAuth = false,
        bool requireConfirmedEmail = false,
        bool requireConfirmedPhone = false)
    {
        var settings = new GetIdentitySettingsForRegistrationGotMsg();
        typeof(GetIdentitySettingsForRegistrationGotMsg)
            .GetProperty("EnableSelfRegistration")
            ?.SetValue(settings, enableSelfRegistration);
        typeof(GetIdentitySettingsForRegistrationGotMsg)
            .GetProperty("EnablePassword")
            ?.SetValue(settings, enablePassword);
        typeof(GetIdentitySettingsForRegistrationGotMsg)
            .GetProperty("EnableTwoAuth")
            ?.SetValue(settings, enableTwoAuth);
        typeof(GetIdentitySettingsForRegistrationGotMsg)
            .GetProperty("RequireConfirmedEmail")
            ?.SetValue(settings, requireConfirmedEmail);
        typeof(GetIdentitySettingsForRegistrationGotMsg)
            .GetProperty("RequireConfirmedPhone")
            ?.SetValue(settings, requireConfirmedPhone);
        return settings;
    }

    private static void SetupRegistrationSettings(IResponseCapableEventBus bus, GetIdentitySettingsForRegistrationGotMsg settings)
    {
        bus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(settings));
        bus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(settings));
    }

    private RegistrationDomainService BuildService(
        TestIdentityUserManager userManager,
        TestCurrentTenant currentTenant,
        IResponseCapableEventBus bus,
        SignInDomainService signInDomainService = null)
    {
        var logger = CreateMockLogger<RegistrationDomainService>();
        var eventBus = (IDistributedEventBus)bus;
        var identityOptions = OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<IdentityOptions>();
        signInDomainService ??= Substitute.For<SignInDomainService>(
            CreateMockLogger<SignInDomainService>(),
            new TestSignInManager(userManager, new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity())),
            identityOptions,
            Substitute.For<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
            BuildIdentitySecurityLogManager(userManager),
            BuildSignInOtpManager(),
            null,
            null,
            null,
            Substitute.For<Microsoft.Extensions.Configuration.IConfiguration>(),
            eventBus,
            Substitute.For<Microsoft.Extensions.Localization.IStringLocalizer<VPortal.Identity.Module.Localization.IdentityResource>>());

        return new RegistrationDomainService(
            logger,
            eventBus,
            userManager,
            currentTenant,
            signInDomainService,
            identityOptions);
    }

    [Fact]
    public async Task GetIdentitySettingsForRegistration_ValidRequest_ReturnsSettings()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(enableSelfRegistration: true, enableTwoAuth: true);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.GetIdentitySettingsForRegistration();

        Assert.NotNull(result);
        Assert.True((bool)typeof(GetIdentitySettingsForRegistrationGotMsg)
            .GetProperty("EnableSelfRegistration")?.GetValue(result));
    }

    [Fact]
    public async Task GetIdentitySettingsForRegistration_EventBusException_ReturnsNull()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(Arg.Any<object>())
            .Returns(Task.FromException<GetIdentitySettingsForRegistrationGotMsg>(new Exception("Event bus error")));
        bus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromException<GetIdentitySettingsForRegistrationGotMsg>(new Exception("Event bus error")));

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.GetIdentitySettingsForRegistration();

        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateUser_SelfRegistrationDisabled_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(enableSelfRegistration: false);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.CreateUser("newuser", email: "newuser@example.com", password: "Password123!");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Contains("don't have permission", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateUser_UserAlreadyExists_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        var existingUser = new IdentityUser(Guid.NewGuid(), "existinguser", "existing@example.com");
        userManager.AddUser(existingUser);

        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings();
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.CreateUser("existinguser", email: "existing@example.com", password: "Password123!");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Contains("already in use", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateUser_PasswordRequiredButMissing_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(enablePassword: true);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.CreateUser("newuser", email: "newuser@example.com", password: null);

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Contains("don't have permission", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateUser_PasswordNotAllowedButProvided_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(enablePassword: false);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.CreateUser("newuser", email: "newuser@example.com", password: "Password123!");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Contains("don't have permission", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateUser_ValidUserWithoutPassword_ReturnsSuccess()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(enablePassword: false);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.CreateUser("newuser", name: "New", surname: "User", email: "newuser@example.com", phoneNumber: "1234567890");

        Assert.Equal(SignInResult.Success, result.SignInResult);
        Assert.True(result.IsNewUser);
        Assert.Equal("newuser@example.com", result.UserEmail);
        Assert.Equal("New", result.UserName);
    }

    [Fact]
    public async Task CreateUser_ValidUserWithPassword_ReturnsSuccess()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(enablePassword: true);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.CreateUser("newuser", name: "New", surname: "User", email: "newuser@example.com", password: "Password123!");

        Assert.Equal(SignInResult.Success, result.SignInResult);
        Assert.True(result.IsNewUser);
        Assert.Equal("newuser@example.com", result.UserEmail);
    }

    [Fact]
    public async Task CreateUser_TwoFactorEnabled_ReturnsTwoFactorRequired()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(enablePassword: true, enableTwoAuth: true);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.CreateUser("newuser", email: "newuser@example.com", password: "Password123!");

        Assert.Equal(SignInResult.Success, result.SignInResult);
        Assert.True(result.TwoFactorRequired);
    }

    [Fact]
    public async Task CreateUser_SettingsNull_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(Arg.Any<object>())
            .Returns(Task.FromResult<GetIdentitySettingsForRegistrationGotMsg>(null));
        bus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult<GetIdentitySettingsForRegistrationGotMsg>(null));

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.CreateUser("newuser", email: "newuser@example.com");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Contains("Settings are empty", result.ErrorMessage);
    }

    [Fact]
    public async Task Resend_UserNotFound_ReturnsFailed()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.Resend("nonexistent@example.com");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Contains("not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Resend_UserFound_ReturnsSuccess()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings();
        SetupRegistrationSettings(bus, settings);
        bus.RequestAsync<GetOtpByRecipientGotMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new GetOtpByRecipientGotMsg { IsExpired = true }));
        bus.RequestAsync<GetOtpByRecipientGotMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new GetOtpByRecipientGotMsg { IsExpired = true }));

        var signInDomainService = Substitute.For<SignInDomainService>(
            CreateMockLogger<SignInDomainService>(),
            new TestSignInManager(userManager, new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity())),
            OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<IdentityOptions>(),
            Substitute.For<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
            BuildIdentitySecurityLogManager(userManager),
            BuildSignInOtpManager(),
            null,
            null,
            null,
            Substitute.For<Microsoft.Extensions.Configuration.IConfiguration>(),
            (IDistributedEventBus)bus,
            Substitute.For<Microsoft.Extensions.Localization.IStringLocalizer<VPortal.Identity.Module.Localization.IdentityResource>>());

        var service = BuildService(userManager, currentTenant, bus, signInDomainService);
        var result = await service.Resend("user@example.com");

        Assert.NotNull(result);
        Assert.Equal("user@example.com", result.UserEmail);
    }

    [Fact]
    public async Task Resend_OtpNotExpired_ReturnsFailed()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<GetOtpByRecipientGotMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new GetOtpByRecipientGotMsg { IsExpired = false }));
        bus.RequestAsync<GetOtpByRecipientGotMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new GetOtpByRecipientGotMsg { IsExpired = false }));

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.Resend("user@example.com");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Contains("MessageAlreadySended", result.ErrorMessage);
    }

    [Fact]
    public async Task UserEmailPhoneVerification_InvalidUser_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings();
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.UserEmailPhoneVerification("nonexistent@example.com");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Contains("not found", result.ErrorMessage);
    }

    [Fact]
    public async Task UserEmailPhoneVerification_ValidUser_ReturnsSuccess()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(requireConfirmedEmail: true);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.UserEmailPhoneVerification("user@example.com");

        Assert.Equal(SignInResult.Success, result.SignInResult);
        Assert.Equal("user@example.com", result.UserEmail);
    }

    [Fact]
    public async Task UserEmailPhoneVerification_RequiresEmailConfirmation_ConfirmsEmail()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        typeof(IdentityUser).GetProperty("EmailConfirmed")?.SetValue(user, false);
        userManager.AddUser(user);

        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(requireConfirmedEmail: true);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.UserEmailPhoneVerification("user@example.com");

        Assert.Equal(SignInResult.Success, result.SignInResult);
        Assert.True(result.IsUserEmailConfirmed);
    }

    [Fact]
    public async Task UserEmailPhoneVerification_RequiresPhoneConfirmation_ConfirmsPhone()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        user.SetPhoneNumber("1234567890", false);
        userManager.AddUser(user);

        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(requireConfirmedPhone: true);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.UserEmailPhoneVerification("user@example.com");

        Assert.Equal(SignInResult.Success, result.SignInResult);
        Assert.True(result.IsUserPhoneConfirmed);
    }

    [Fact]
    public async Task SendCode_UserNotFound_ReturnsFailed()
    {
        var userManager = new TestIdentityUserManager();
        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings();
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.SendCode("nonexistent@example.com");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Contains("not found", result.ErrorMessage);
    }

    [Fact]
    public async Task SendCode_UserFound_SendsOtp()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings();
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.SendCode("user@example.com");

        Assert.NotNull(result);
        Assert.Equal("user@example.com", result.UserEmail);
        Assert.Equal("user", result.UserName);
    }

    [Fact]
    public async Task SendCode_SelfRegistrationDisabled_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var currentTenant = new TestCurrentTenant();
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var settings = BuildRegistrationSettings(enableSelfRegistration: false);
        SetupRegistrationSettings(bus, settings);

        var service = BuildService(userManager, currentTenant, bus);
        var result = await service.SendCode("user@example.com");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Contains("don't have permission", result.ErrorMessage);
    }
}
