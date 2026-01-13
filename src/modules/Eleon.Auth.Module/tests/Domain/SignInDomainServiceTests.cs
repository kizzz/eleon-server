using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.EventBus.Module;
using Common.Module.Constants;
using EleonsoftAbp.EleonsoftIdentity.Sessions;
using EleonsoftModuleCollector.Commons.Module.Messages.Identity;
using ExternalLogin.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using NSubstitute;
using Newtonsoft.Json;
using System.Runtime.ExceptionServices;
using TenantSettings.Module.Cache;
using TenantSettings.Module.Messaging;
using TenantSettings.Module.Models;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.SecurityLog;
using Volo.Abp.Users;
using VPortal.Identity.Module.DomainServices;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Sessions;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using AbpIdentityResource = Volo.Abp.Identity.Localization.IdentityResource;
using VPortalIdentityResource = VPortal.Identity.Module.Localization.IdentityResource;
using Logging.Module;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class SignInDomainServiceTests : DomainTestBase
{
    private static (IVportalLogger<SignInDomainService> logger, Func<Exception> getException) CreateCapturingLogger()
    {
        Exception captured = null;
        var logger = Substitute.For<IVportalLogger<SignInDomainService>>();
        var standardLogger = Substitute.For<Microsoft.Extensions.Logging.ILogger>();
        logger.Log.Returns(standardLogger);
        logger.When(l => l.Capture(Arg.Any<Exception>(), Arg.Any<string>()))
            .Do(callInfo => captured = callInfo.Arg<Exception>());
        logger.When(l => l.Capture(Arg.Any<Exception>()))
            .Do(callInfo => captured = callInfo.Arg<Exception>());
        return (logger, () => captured);
    }
    private sealed class StubSessionAccessor(FullSessionInformation session) : ISessionAccessor
    {
        public FullSessionInformation Session { get; } = session;
    }

    private static ExternalLoginInfo BuildExternalLoginInfo(string email, string provider = null)
    {
        List<Claim> claims = [];
        if (!string.IsNullOrWhiteSpace(email))
        {
            claims.Add(new Claim(AbpClaimTypes.Email, email));
        }

        var identity = new ClaimsIdentity(claims, "external");
        var principal = new ClaimsPrincipal(identity);
        provider ??= ExternalLoginProviderType.Local.ToString();
        return new ExternalLoginInfo(principal, provider, "key", provider);
    }

    private static IdentitySecurityLogManager BuildSecurityLogManager(TestIdentityUserManager userManager, ISecurityLogManager securityLogManager)
    {
        var principalAccessor = Substitute.For<ICurrentPrincipalAccessor>();
        principalAccessor.Change(Arg.Any<ClaimsPrincipal>()).Returns(Substitute.For<IDisposable>());

        var principalFactory = Substitute.For<IUserClaimsPrincipalFactory<IdentityUser>>();
        principalFactory.CreateAsync(Arg.Any<IdentityUser>()).Returns(Task.FromResult(new ClaimsPrincipal()));

        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(true);

        return new IdentitySecurityLogManager(securityLogManager, userManager, principalAccessor, principalFactory, currentUser);
    }

    private static TenantSettingsCacheService BuildTenantSettingsCacheService(TestCurrentTenant currentTenant, TenantStatus status)
    {
        var bus = EventBusTestHelpers.CreateMockResponseCapableEventBus();
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

    private static SignInOtpManager BuildOtpManager(IDistributedEventBus bus)
    {
        var localizer = Substitute.For<IStringLocalizer<AbpIdentityResource>>();
        localizer["OtpMessage"].Returns(new LocalizedString("OtpMessage", "Otp message"));

        return new SignInOtpManager(
            bus,
            localizer,
            new ConfigurationBuilder().Build(),
            new StubSessionAccessor(new FullSessionInformation { SessionId = "session" }));
    }

    private SignInDomainService BuildService(
        TestIdentityUserManager userManager,
        TestSignInManager signInManager,
        TestCurrentTenant currentTenant,
        DefaultHttpContext httpContext,
        IDistributedEventBus eventBus,
        ISecurityLogManager securityLogManager,
        TenantSettingsCacheService tenantSettingsCache,
        IStringLocalizer<VPortalIdentityResource> localizer,
        Logging.Module.IVportalLogger<SignInDomainService> logger = null)
    {
        var authService = httpContext.RequestServices.GetRequiredService<IAuthenticationService>();
        _ = authService;

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var identityOptions = OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<IdentityOptions>();
        var guidGenerator = CreateMockGuidGenerator();
        var signInOtpManager = BuildOtpManager(eventBus);
        var identitySecurityLogManager = BuildSecurityLogManager(userManager, securityLogManager);

        var externalLoginManager = new ExternalLoginManager(
            signInManager,
            userManager,
            guidGenerator,
            currentTenant,
            identityOptions,
            httpContextAccessor,
            tenantSettingsCache,
            identitySecurityLogManager);

        return new SignInDomainService(
            logger ?? CreateMockLogger<SignInDomainService>(),
            signInManager,
            identityOptions,
            httpContextAccessor,
            identitySecurityLogManager,
            signInOtpManager,
            null,
            externalLoginManager,
            null,
            new ConfigurationBuilder().Build(),
            eventBus,
            localizer);
    }

    private static DefaultHttpContext BuildHttpContext(TestAuthenticationService authService, TestCurrentTenant currentTenant, IExternalLoginOptionsConfigurator configurator)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(authService);
        services.AddSingleton<ICurrentTenant>(currentTenant);
        services.AddSingleton<IExternalLoginOptionsConfigurator>(configurator);
        var provider = services.BuildServiceProvider();

        return new DefaultHttpContext { RequestServices = provider };
    }

    private static TestExternalLoginOptionsConfigurator BuildOptionsConfigurator(Guid? tenantId, string privateKey)
    {
        return new TestExternalLoginOptionsConfigurator(new Dictionary<string, string>
        {
            { tenantId?.ToString() ?? "host", privateKey }
        });
    }

    private sealed class TestExternalLoginOptionsConfigurator(Dictionary<string, string> keys) : IExternalLoginOptionsConfigurator
    {
        public void ConfigureOptions(string authenticationSchemeName, Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions options)
        {
            var key = keys.GetValueOrDefault("host");
            options.ClientSecret = key;
        }
    }

    private static GetIdentitySettingsForRegistrationGotMsg BuildRegistrationSettings(bool enableTwoAuth)
    {
        var settings = new GetIdentitySettingsForRegistrationGotMsg();
        typeof(GetIdentitySettingsForRegistrationGotMsg)
            .GetProperty("EnableTwoAuth")
            ?.SetValue(settings, enableTwoAuth);
        return settings;
    }

    private static void SetupTwoFactorSettings(IResponseCapableEventBus bus, bool enableTwoAuth)
    {
        bus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(BuildRegistrationSettings(enableTwoAuth)));
        bus.RequestAsync<GetIdentitySettingsForRegistrationGotMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(BuildRegistrationSettings(enableTwoAuth)));

        bus.RequestAsync<IdentitySettingsResponseMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new IdentitySettingsResponseMsg
            {
                Settings =
                [
                    new() { Name = IdentitySettingsConsts.TwoFactorAuthenticationOption, Value = IdentitySettingsConsts.TwoFactorAuthenticationOptions.Email }
                ]
            }));
        bus.RequestAsync<IdentitySettingsResponseMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new IdentitySettingsResponseMsg
            {
                Settings =
                [
                    new() { Name = IdentitySettingsConsts.TwoFactorAuthenticationOption, Value = IdentitySettingsConsts.TwoFactorAuthenticationOptions.Email }
                ]
            }));
    }

    private static void SetupOtpGeneration(IResponseCapableEventBus bus, bool success, string message)
    {
        bus.RequestAsync<OtpSentMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new OtpSentMsg
            {
                Result = new() { Success = success, Message = message }
            }));
        bus.RequestAsync<OtpSentMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new OtpSentMsg
            {
                Result = new() { Success = success, Message = message }
            }));
    }

    private static void SetupOtpValidation(IResponseCapableEventBus bus, bool valid, string errorMessage)
    {
        bus.RequestAsync<OtpValidatedMsg>(Arg.Any<object>())
            .Returns(Task.FromResult(new OtpValidatedMsg
            {
                Result = new OtpValidationResultEto(valid, errorMessage)
            }));
        bus.RequestAsync<OtpValidatedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new OtpValidatedMsg
            {
                Result = new OtpValidationResultEto(valid, errorMessage)
            }));
    }

    [Fact]
    public async Task SignInWithOtpAsync_NoExternalLogin_ReturnsFailedWithMessage()
    {
        var userManager = new TestIdentityUserManager();
        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = null;

        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();
        localizer["Error:ExternalLoginInfo:NotFound"].Returns(new LocalizedString("Error:ExternalLoginInfo:NotFound", "Missing external login"));

        var bus = CreateMockResponseCapableEventBus();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(null, "key"));

        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer);

        var result = await service.SignInWithOtpAsync();

        Assert.Equal(SignInResult.Failed, result.SignInResult);
        Assert.Equal("Missing external login", result.ErrorMessage);
    }

    [Fact]
    public async Task SignInWithOtpAsync_TwoFactorEnabled_NoOtp_ReturnsTwoFactorRequired()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = BuildExternalLoginInfo(user.Email, ExternalLoginProviderType.Local.ToString());

        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

        var bus = CreateMockResponseCapableEventBus();
        SetupTwoFactorSettings((IResponseCapableEventBus)bus, enableTwoAuth: true);
        SetupOtpGeneration((IResponseCapableEventBus)bus, success: true, message: "Otp sent");

        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(null, "key"));

        var (logger, getException) = CreateCapturingLogger();
        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer, logger);

        var result = await service.SignInWithOtpAsync();
        if (result == null)
        {
            var ex = getException();
            if (ex != null)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        Assert.Equal(SignInResult.TwoFactorRequired, result.SignInResult);
        Assert.Equal("use**********com", result.SuccessMessage);
    }

    [Fact]
    public async Task SignInWithOtpAsync_InvalidOtp_ReturnsTwoFactorRequiredWithError()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = BuildExternalLoginInfo(user.Email, ExternalLoginProviderType.Local.ToString());

        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

        var bus = CreateMockResponseCapableEventBus();
        SetupTwoFactorSettings((IResponseCapableEventBus)bus, enableTwoAuth: true);
        SetupOtpValidation((IResponseCapableEventBus)bus, valid: false, errorMessage: "invalid otp");

        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(null, "key"));

        var (logger, getException) = CreateCapturingLogger();
        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer, logger);

        var result = await service.SignInWithOtpAsync("000000");
        if (result == null)
        {
            var ex = getException();
            if (ex != null)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        Assert.Equal(SignInResult.TwoFactorRequired, result.SignInResult);
        Assert.Equal("invalid otp", result.ErrorMessage);
    }

    [Fact]
    public async Task SignInWithOtpAsync_InactiveTenant_ReturnsLockedOut()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = BuildExternalLoginInfo(user.Email, ExternalLoginProviderType.Local.ToString());

        var currentTenant = new TestCurrentTenant();
        using (currentTenant.Change(Guid.NewGuid(), "tenant"))
        {
            var authService = new TestAuthenticationService();
            var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

            var bus = CreateMockResponseCapableEventBus();
            SetupTwoFactorSettings((IResponseCapableEventBus)bus, enableTwoAuth: true);

            var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Suspended);
            var securityLogManager = Substitute.For<ISecurityLogManager>();
            securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

            var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(currentTenant.Id, "key"));

            var (logger, getException) = CreateCapturingLogger();
            var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer, logger);

            var result = await service.SignInWithOtpAsync();
            if (result == null)
            {
                var ex = getException();
                if (ex != null)
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }

            Assert.Equal(SignInResult.LockedOut, result.SignInResult);
        }
    }

    [Fact]
    public async Task SignInWithOtpAsync_TwoFactorDisabled_ReturnsSuccess()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = BuildExternalLoginInfo(user.Email, ExternalLoginProviderType.Local.ToString());

        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

        var bus = CreateMockResponseCapableEventBus();
        SetupTwoFactorSettings((IResponseCapableEventBus)bus, enableTwoAuth: false);

        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(null, "key"));

        var (logger, getException) = CreateCapturingLogger();
        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer, logger);

        var result = await service.SignInWithOtpAsync();
        if (result == null)
        {
            var ex = getException();
            if (ex != null)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        Assert.Equal(SignInResult.Success, result.SignInResult);
        Assert.Equal(IdentityConstants.ExternalScheme, authService.SignedOutScheme);
    }

    [Fact]
    public async Task SignInWithOtpAsync_ValidOtp_ReturnsSuccess()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = BuildExternalLoginInfo(user.Email, ExternalLoginProviderType.Local.ToString());

        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

        var bus = CreateMockResponseCapableEventBus();
        SetupTwoFactorSettings((IResponseCapableEventBus)bus, enableTwoAuth: true);
        SetupOtpValidation((IResponseCapableEventBus)bus, valid: true, errorMessage: null);

        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(null, "key"));

        var (logger, getException) = CreateCapturingLogger();
        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer, logger);

        var result = await service.SignInWithOtpAsync("123456");
        if (result == null)
        {
            var ex = getException();
            if (ex != null)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        Assert.Equal(SignInResult.Success, result.SignInResult);
    }

    [Fact]
    public async Task SignInWithOtpAsync_RegistrationProcess_HandlesDifferently()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = BuildExternalLoginInfo(user.Email, ExternalLoginProviderType.Local.ToString());

        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

        var bus = CreateMockResponseCapableEventBus();
        SetupTwoFactorSettings((IResponseCapableEventBus)bus, enableTwoAuth: true);
        SetupOtpValidation((IResponseCapableEventBus)bus, valid: true, errorMessage: null);

        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(null, "key"));

        var (logger, getException) = CreateCapturingLogger();
        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer, logger);

        var result = await service.SignInWithOtpAsync("123456", isRegistrationProcess: true);
        if (result == null)
        {
            var ex = getException();
            if (ex != null)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        Assert.NotNull(result);
    }

    [Fact]
    public async Task SignInWithOtpAsync_UserLockedOut_ReturnsLockedOut()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = BuildExternalLoginInfo(user.Email, ExternalLoginProviderType.Local.ToString());

        var currentTenant = new TestCurrentTenant();
        currentTenant.Change(Guid.NewGuid());
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

        var bus = CreateMockResponseCapableEventBus();
        SetupTwoFactorSettings((IResponseCapableEventBus)bus, enableTwoAuth: true);
        SetupOtpGeneration((IResponseCapableEventBus)bus, success: true, message: "test");

        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Suspended);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(currentTenant.Id, "key"));

        var (logger, getException) = CreateCapturingLogger();
        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer, logger);

        var result = await service.SignInWithOtpAsync();
        if (result == null)
        {
            var ex = getException();
            if (ex != null)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        Assert.Equal(SignInResult.LockedOut, result.SignInResult);
    }

    [Fact]
    public async Task HasExternalLogin_NoExternalLogin_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = null;

        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

        var bus = CreateMockResponseCapableEventBus();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(null, "key"));

        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer);

        var result = await service.HasExternalLogin();

        Assert.False(result);
    }

    [Fact]
    public async Task HasExternalLogin_ExternalLoginExists_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = BuildExternalLoginInfo(user.Email, ExternalLoginProviderType.Local.ToString());

        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

        var bus = CreateMockResponseCapableEventBus();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(null, "key"));

        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer);

        var result = await service.HasExternalLogin();

        Assert.True(result);
    }

    [Fact]
    public async Task ClearExternalLogin_RemovesExternalLogin()
    {
        var userManager = new TestIdentityUserManager();
        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        signInManager.ExternalLoginInfo = BuildExternalLoginInfo("user@example.com");

        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

        var bus = CreateMockResponseCapableEventBus();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(null, "key"));
        // Add the external cookie before clearing it
        httpContext.Response.Cookies.Append(IdentityConstants.ExternalScheme, "test-value");

        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer);

        await service.ClearExternalLogin();

        // ClearExternalLogin deletes the cookie by issuing an expired cookie header
        var cookie = httpContext.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains(IdentityConstants.ExternalScheme, cookie);
        Assert.Contains("expires", cookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetExternalLoginInfo_ReturnsLoginInfo()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var expectedLoginInfo = BuildExternalLoginInfo(user.Email, ExternalLoginProviderType.Local.ToString());
        signInManager.ExternalLoginInfo = expectedLoginInfo;

        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService();
        var localizer = Substitute.For<IStringLocalizer<VPortalIdentityResource>>();

        var bus = CreateMockResponseCapableEventBus();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var httpContext = BuildHttpContext(authService, currentTenant, BuildOptionsConfigurator(null, "key"));

        var service = BuildService(userManager, signInManager, currentTenant, httpContext, (IDistributedEventBus)bus, securityLogManager, tenantSettingsCache, localizer);

        var result = await service.GetExternalLoginInfo();

        Assert.NotNull(result);
        Assert.Equal(expectedLoginInfo.LoginProvider, result.LoginProvider);
    }
}
