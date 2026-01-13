using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.EventBus.Module;
using Common.Module.Constants;
using Common.Module.Keys;
using EleonsoftAbp.EleonsoftIdentity.Sessions;
using EleonsoftModuleCollector.Commons.Module.Messages.Identity;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Sessions;
using Migrations.Module;
using ExternalLogin.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Eleon.TestsBase.Lib.TestHelpers;
using TenantSettings.Module.Cache;
using TenantSettings.Module.Messaging;
using TenantSettings.Module.Models;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using VPortal.Identity.Module.DomainServices;
using VPortal.Identity.Module.CustomCredentials;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Newtonsoft.Json;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class LocalSignInDomainServiceTests : DomainTestBase
{
    private static GetIdentitySettingsForRegistrationGotMsg BuildRegistrationSettings(
        bool enablePassword = true,
        bool enableTwoAuth = false)
    {
        var settings = new GetIdentitySettingsForRegistrationGotMsg();
        typeof(GetIdentitySettingsForRegistrationGotMsg)
            .GetProperty("EnablePassword")
            ?.SetValue(settings, enablePassword);
        typeof(GetIdentitySettingsForRegistrationGotMsg)
            .GetProperty("EnableTwoAuth")
            ?.SetValue(settings, enableTwoAuth);
        return settings;
    }

    private static TenantSettingsCacheService BuildTenantSettingsCacheService(TestCurrentTenant currentTenant, TenantStatus status)
    {
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        var tenantId = currentTenant.Id ?? Guid.NewGuid();

        var eto = new TenantSettingsCacheEto
        {
            TenantSettings =
            [
                new TenantSetting
                {
                    TenantId = tenantId,
                    Status = status
                }
            ],
            UserIsolationSettings = [],
            HostAdminUsers = [],
            Cors = []
        };

        var json = JsonConvert.SerializeObject(eto);
        bus.RequestAsync<TenantSettingsGotMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new TenantSettingsGotMsg { SettingsJson = json }));

        var options = Options.Create(new TenantSettingsCacheOptions());
        var multiOptions = Options.Create(new SharedModule.modules.MultiTenancy.Module.EleonMultiTenancyOptions());
        var tenantCacheService = new TenantCacheService((IDistributedEventBus)bus, currentTenant, multiOptions);
        return new TenantSettingsCacheService(currentTenant, options, (IDistributedEventBus)bus, tenantCacheService, multiOptions);
    }

    private ExternalLoginManager BuildExternalLoginManager(
        TestIdentityUserManager userManager,
        TestSignInManager signInManager,
        TestCurrentTenant currentTenant,
        DefaultHttpContext httpContext,
        TenantStatus status)
    {
        if (status != TenantStatus.Active && currentTenant.Id == null)
        {
            currentTenant.Change(Guid.NewGuid());
        }

        var identityOptions = OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<IdentityOptions>();
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, status);

        return new ExternalLoginManager(
            signInManager,
            userManager,
            CreateMockGuidGenerator(),
            currentTenant,
            identityOptions,
            httpContextAccessor,
            tenantSettingsCache,
            BuildIdentitySecurityLogManager(userManager));
    }

    private RegistrationDomainService BuildRegistrationDomainService(
        TestIdentityUserManager userManager,
        TestCurrentTenant currentTenant,
        GetIdentitySettingsForRegistrationGotMsg settings)
    {
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        if (settings == null)
        {
            bus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(Arg.Any<object>())
                .Returns(Task.FromResult<GetIdentitySettingsForRegistrationGotMsg>(null));
            bus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(Arg.Any<object>(), Arg.Any<int>())
                .Returns(Task.FromResult<GetIdentitySettingsForRegistrationGotMsg>(null));
        }
        else
        {
            SetupEventBusRequestAsync<GetIdentitySettingsForRegistrationMsg, GetIdentitySettingsForRegistrationGotMsg>(bus, settings);
        }

        var identityOptions = OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<IdentityOptions>();
        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);

        return new RegistrationDomainService(
            CreateMockLogger<RegistrationDomainService>(),
            (IDistributedEventBus)bus,
            userManager,
            currentTenant,
            signInDomainService,
            identityOptions);
    }

    private SignInDomainService BuildSignInDomainService(
        TestIdentityUserManager userManager,
        TestSignInManager signInManager,
        TestCurrentTenant currentTenant,
        DefaultHttpContext httpContext,
        ExternalLoginManager externalLoginManager)
    {
        var identityOptions = OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<IdentityOptions>();
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var localizer = Substitute.For<Microsoft.Extensions.Localization.IStringLocalizer<VPortal.Identity.Module.Localization.IdentityResource>>();
        var eventBus = CreateMockEventBus();

        return new SignInDomainService(
            CreateMockLogger<SignInDomainService>(),
            signInManager,
            identityOptions,
            httpContextAccessor,
            BuildIdentitySecurityLogManager(userManager),
            BuildConfiguredSignInOtpManager(),
            null,
            externalLoginManager,
            null,
            new ConfigurationBuilder().Build(),
            eventBus,
            localizer);
    }

    private sealed class StubSessionAccessor(FullSessionInformation session) : ISessionAccessor
    {
        public FullSessionInformation Session { get; } = session;
    }

    private IResponseCapableEventBus BuildOtpBus(string option = IdentitySettingsConsts.TwoFactorAuthenticationOptions.Email)
    {
        var bus = Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
        bus.RequestAsync<IdentitySettingsResponseMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new IdentitySettingsResponseMsg
            {
                Settings =
                [
                    new() { Name = IdentitySettingsConsts.TwoFactorAuthenticationOption, Value = option }
                ]
            }));
        bus.RequestAsync<OtpSentMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new OtpSentMsg
            {
                Result = new OtpGenerationResultDto { Success = true, Message = string.Empty }
            }));
        return bus;
    }

    private SignInOtpManager BuildConfiguredSignInOtpManager(IResponseCapableEventBus bus = null)
    {
        var localizer = Substitute.For<Microsoft.Extensions.Localization.IStringLocalizer<Volo.Abp.Identity.Localization.IdentityResource>>();
        localizer["OtpMessage"].Returns(new Microsoft.Extensions.Localization.LocalizedString("OtpMessage", "Otp message"));
        var configuration = new ConfigurationBuilder().Build();
        var sessionAccessor = new StubSessionAccessor(new FullSessionInformation { SessionId = "session" });
        bus ??= BuildOtpBus();

        return new SignInOtpManager(
            (IDistributedEventBus)bus,
            localizer,
            configuration,
            sessionAccessor);
    }

    private LocalSignInDomainService BuildService(
        TestIdentityUserManager userManager,
        TestSignInManager signInManager,
        TestCurrentTenant currentTenant,
        ExternalLoginManager externalLoginManager,
        RegistrationDomainService registrationDomainService,
        SignInDomainService signInDomainService,
        SignInOtpManager signInOtpManager,
        IIdentitySecurityLogRepository securityLogRepository,
        DefaultHttpContext httpContext)
    {
        var logger = CreateMockLogger<LocalSignInDomainService>();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };

        return new LocalSignInDomainService(
            logger,
            signInManager,
            externalLoginManager,
            tenantSettingsCache,
            userManager,
            securityLogRepository,
            registrationDomainService,
            signInDomainService,
            signInOtpManager,
            httpContextAccessor);
    }

    [Fact]
    public async Task SignInAsync_UserNotFound_ReturnsFailed()
    {
        var userManager = new TestIdentityUserManager();
        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, BuildRegistrationSettings());
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        var signInOtpManager = BuildConfiguredSignInOtpManager();

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();
        securityLogRepository.GetListAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.DateTime?>(), Arg.Any<System.DateTime?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<System.Guid?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(Task.FromResult(new List<IdentitySecurityLog>()));

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        var result = await service.SignInAsync("nonexistent");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
    }

    [Fact]
    public async Task SignInAsync_UserLockedOut_ReturnsLockedOut()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Suspended);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, BuildRegistrationSettings());
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        var signInOtpManager = BuildConfiguredSignInOtpManager();

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        var result = await service.SignInAsync("user");

        Assert.Equal(SignInResult.LockedOut, result.SignInResult);
    }

    [Fact]
    public async Task SignInAsync_SettingsNull_ReturnsFailed()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, null);
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        var signInOtpManager = BuildConfiguredSignInOtpManager();

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        var result = await service.SignInAsync("user");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Equal("Settings are empty", result.ErrorMessage);
    }

    [Fact]
    public async Task SignInAsync_PasswordRequiredButMissing_ReturnsFailed()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, BuildRegistrationSettings(enablePassword: true));
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        var signInOtpManager = BuildConfiguredSignInOtpManager();

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        var result = await service.SignInAsync("user", password: null);

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Equal("Password is required", result.ErrorMessage);
    }

    [Fact]
    public async Task SignInAsync_InvalidPassword_ReturnsFailed()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        userManager.SetPasswordCheckResult(false);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, BuildRegistrationSettings(enablePassword: true));
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        var signInOtpManager = BuildConfiguredSignInOtpManager();

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        var result = await service.SignInAsync("user", password: "wrongpassword");

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Equal("Password is invalid", result.ErrorMessage);
    }

    [Fact]
    public async Task SignInAsync_TwoFactorEnabled_ReturnsTwoFactorRequired()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        userManager.SetPasswordCheckResult(true);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, BuildRegistrationSettings(enablePassword: true, enableTwoAuth: true));
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        signInManager.ExternalLoginInfo = null;

        var otpBus = BuildOtpBus();
        var signInOtpManager = BuildConfiguredSignInOtpManager(otpBus);

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();
        securityLogRepository.GetListAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.DateTime?>(), Arg.Any<System.DateTime?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<System.Guid?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(Task.FromResult(new List<IdentitySecurityLog>()));

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        var result = await service.SignInAsync("user", password: "correctpassword");

        Assert.Equal(SignInResult.TwoFactorRequired, result.SignInResult);
        Assert.False(string.IsNullOrWhiteSpace(result.SuccessMessage));
    }

    [Fact]
    public async Task SignInAsync_TwoFactorEnabled_SendsOtp()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        userManager.SetPasswordCheckResult(true);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, BuildRegistrationSettings(enablePassword: true, enableTwoAuth: true));
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        signInManager.ExternalLoginInfo = null;

        var otpBus = BuildOtpBus();
        var signInOtpManager = BuildConfiguredSignInOtpManager(otpBus);

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();
        securityLogRepository.GetListAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.DateTime?>(), Arg.Any<System.DateTime?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<System.Guid?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(Task.FromResult(new List<IdentitySecurityLog>()));

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        await service.SignInAsync("user", password: "correctpassword");

        await otpBus.Received().RequestAsync<OtpSentMsg>(Arg.Any<object>(), Arg.Any<int>());
    }

    [Fact]
    public async Task SignInAsync_TwoFactorDisabled_ReturnsSuccess()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        userManager.SetPasswordCheckResult(true);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, BuildRegistrationSettings(enablePassword: true, enableTwoAuth: false));
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        var signInOtpManager = BuildConfiguredSignInOtpManager();

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();
        securityLogRepository.GetListAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.DateTime?>(), Arg.Any<System.DateTime?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<System.Guid?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(Task.FromResult(new List<IdentitySecurityLog>()));

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        var result = await service.SignInAsync("user", password: "correctpassword");

        Assert.Equal(SignInResult.Success, result.SignInResult);
        Assert.Equal("user@example.com", result.UserEmail);
    }

    [Fact]
    public async Task SignInAsync_TwoFactorDisabled_SignsInWithClaims()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        userManager.SetPasswordCheckResult(true);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, BuildRegistrationSettings(enablePassword: true, enableTwoAuth: false));
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        var signInOtpManager = BuildConfiguredSignInOtpManager();

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();
        securityLogRepository.GetListAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.DateTime?>(), Arg.Any<System.DateTime?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<System.Guid?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(Task.FromResult(new List<IdentitySecurityLog>()));

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        await service.SignInAsync("user", password: "correctpassword");

        Assert.NotEmpty(signInManager.CapturedClaims);
        Assert.Contains(signInManager.CapturedClaims, c => c.Type == "idp" && c.Value == "local");
        Assert.Contains(signInManager.CapturedClaims, c => c.Type == "amr" && c.Value == "local");
    }

    [Fact]
    public async Task SignInAsync_NewUser_IsNewUserFlagSet()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        userManager.SetPasswordCheckResult(true);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, BuildRegistrationSettings(enablePassword: true, enableTwoAuth: false));
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        var signInOtpManager = BuildConfiguredSignInOtpManager();

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();
        securityLogRepository.GetListAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.DateTime?>(), Arg.Any<System.DateTime?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<System.Guid?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(Task.FromResult(new List<IdentitySecurityLog>()));

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        var result = await service.SignInAsync("user", password: "correctpassword");

        Assert.True(result.IsNewUser);
    }

    [Fact]
    public async Task SignInAsync_ExistingUser_IsNewUserFlagNotSet()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        userManager.SetPasswordCheckResult(true);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var httpContext = new DefaultHttpContext();
        var externalLoginManager = BuildExternalLoginManager(userManager, signInManager, currentTenant, httpContext, TenantStatus.Active);
        var registrationDomainService = BuildRegistrationDomainService(userManager, currentTenant, BuildRegistrationSettings(enablePassword: true, enableTwoAuth: false));
        var signInDomainService = BuildSignInDomainService(userManager, signInManager, currentTenant, httpContext, externalLoginManager);
        var signInOtpManager = BuildConfiguredSignInOtpManager();

        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();
        var existingLog = Substitute.For<IdentitySecurityLog>();
        typeof(IdentitySecurityLog).GetProperty("UserId")?.SetValue(existingLog, user.Id);
        securityLogRepository.GetListAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.DateTime?>(), Arg.Any<System.DateTime?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<System.Guid?>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(Task.FromResult(new List<IdentitySecurityLog> { existingLog }));

        var service = BuildService(userManager, signInManager, currentTenant, externalLoginManager,
            registrationDomainService, signInDomainService, signInOtpManager, securityLogRepository, httpContext);

        var result = await service.SignInAsync("user", password: "correctpassword");

        Assert.False(result.IsNewUser);
    }
}
