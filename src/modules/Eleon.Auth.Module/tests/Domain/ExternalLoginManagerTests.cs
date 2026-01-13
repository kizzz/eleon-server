using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.Module.Constants;
using Migrations.Module;
using ExternalLogin.Module;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using TenantSettings.Module.Cache;
using TenantSettings.Module.Messaging;
using TenantSettings.Module.Models;
using Volo.Abp;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Eleon.TestsBase.Lib.TestHelpers;
using Volo.Abp.Security.Claims;
using Volo.Abp.SecurityLog;
using Volo.Abp.Users;
using VPortal.Identity.Module.DomainServices;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Newtonsoft.Json;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class ExternalLoginManagerTests : DomainTestBase
{
    private static ExternalLoginInfo BuildExternalLoginInfo(string email, string provider = "Google")
    {
        var claims = new List<Claim>();
        if (!string.IsNullOrWhiteSpace(email))
        {
            claims.Add(new Claim(AbpClaimTypes.Email, email));
            claims.Add(new Claim(AbpClaimTypes.UserName, email.Split('@')[0]));
            claims.Add(new Claim(AbpClaimTypes.Name, email.Split('@')[0]));
            claims.Add(new Claim(ClaimTypes.Name, email));
            claims.Add(new Claim(ClaimTypes.Email, email));
        }
        claims.Add(new Claim(ClaimTypes.NameIdentifier, "provider-key-123"));

        var identity = new ClaimsIdentity(claims, "external", ClaimTypes.Email, ClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);
        return new ExternalLoginInfo(principal, provider, "provider-key-123", provider);
    }

    private static TenantSettingsCacheService BuildTenantSettingsCacheService(TestCurrentTenant currentTenant, TenantStatus status)
    {
        var bus = Substitute.For<Common.EventBus.Module.IResponseCapableEventBus, Volo.Abp.EventBus.Distributed.IDistributedEventBus>();
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
        bus.RequestAsync<Messaging.Module.Messages.TenantSettingsGotMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new Messaging.Module.Messages.TenantSettingsGotMsg { SettingsJson = json }));

        var options = Options.Create(new TenantSettingsCacheOptions());
        var multiOptions = Options.Create(new SharedModule.modules.MultiTenancy.Module.EleonMultiTenancyOptions());
        var tenantCacheService = new TenantCacheService((Volo.Abp.EventBus.Distributed.IDistributedEventBus)bus, currentTenant, multiOptions);
        return new TenantSettingsCacheService(currentTenant, options, (Volo.Abp.EventBus.Distributed.IDistributedEventBus)bus, tenantCacheService, multiOptions);
    }

    private ExternalLoginManager BuildService(
        TestIdentityUserManager userManager,
        TestSignInManager signInManager,
        TestCurrentTenant currentTenant,
        TenantSettingsCacheService tenantSettingsCache,
        DefaultHttpContext httpContext)
    {
        var guidGenerator = CreateMockGuidGenerator();
        var identityOptions = CreateTestAbpDynamicOptionsManager<IdentityOptions>();
        
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var identitySecurityLogManager = BuildIdentitySecurityLogManager(userManager);

        return new ExternalLoginManager(
            signInManager,
            userManager,
            guidGenerator,
            currentTenant,
            identityOptions,
            httpContextAccessor,
            tenantSettingsCache,
            identitySecurityLogManager);
    }

    [Fact]
    public async Task GetOrCreateUser_UserExists_ReturnsExistingUser()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var httpContext = new DefaultHttpContext();

        var service = BuildService(userManager, signInManager, currentTenant, tenantSettingsCache, httpContext);
        var loginInfo = BuildExternalLoginInfo("user@example.com");

        var result = await service.GetOrCreateUser(loginInfo);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetOrCreateUser_UserNotExists_CreatesNewUser()
    {
        var userManager = new TestIdentityUserManager();
        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var httpContext = new DefaultHttpContext();

        var service = BuildService(userManager, signInManager, currentTenant, tenantSettingsCache, httpContext);
        var loginInfo = BuildExternalLoginInfo("newuser@example.com", "Google");

        var result = await service.GetOrCreateUser(loginInfo);

        Assert.NotNull(result);
        Assert.Equal("newuser@example.com", result.Email);
    }

    [Fact]
    public async Task GetOrCreateUser_ExternalProvider_AddsLogin()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var httpContext = new DefaultHttpContext();

        var service = BuildService(userManager, signInManager, currentTenant, tenantSettingsCache, httpContext);
        var loginInfo = BuildExternalLoginInfo("user@example.com", "Google");

        userManager.SetFindByLoginResult(null);

        var result = await service.GetOrCreateUser(loginInfo);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetOrCreateUser_LocalProvider_DoesNotAddLogin()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var httpContext = new DefaultHttpContext();

        var service = BuildService(userManager, signInManager, currentTenant, tenantSettingsCache, httpContext);
        var loginInfo = BuildExternalLoginInfo("user@example.com", "Local");

        var result = await service.GetOrCreateUser(loginInfo);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task IsUserLockedOut_HostTenant_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var httpContext = new DefaultHttpContext();

        var service = BuildService(userManager, signInManager, currentTenant, tenantSettingsCache, httpContext);

        var result = await service.IsUserLockedOut(user);

        Assert.False(result);
    }

    [Fact]
    public async Task IsUserLockedOut_AdminUser_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "admin", "admin@example.com");
        userManager.AddUser(user);
        await userManager.AddToRoleAsync(user, MigrationConsts.AdminRoleNameDefaultValue);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        using (currentTenant.Change(Guid.NewGuid(), "tenant"))
        {
            var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Suspended);
            var httpContext = new DefaultHttpContext();

            var service = BuildService(userManager, signInManager, currentTenant, tenantSettingsCache, httpContext);

            var result = await service.IsUserLockedOut(user);

            Assert.False(result);
        }
    }

    [Fact]
    public async Task IsUserLockedOut_TenantSuspended_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        using (currentTenant.Change(Guid.NewGuid(), "tenant"))
        {
            var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Suspended);
            var httpContext = new DefaultHttpContext();

            var service = BuildService(userManager, signInManager, currentTenant, tenantSettingsCache, httpContext);

            var result = await service.IsUserLockedOut(user);

            Assert.True(result);
        }
    }

    [Fact]
    public async Task IsUserLockedOut_TenantActive_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        using (currentTenant.Change(Guid.NewGuid(), "tenant"))
        {
            var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
            var httpContext = new DefaultHttpContext();

            var service = BuildService(userManager, signInManager, currentTenant, tenantSettingsCache, httpContext);

            var result = await service.IsUserLockedOut(user);

            Assert.False(result);
        }
    }

    [Fact]
    public async Task SignInHttpContext_ValidUser_SignsInWithClaims()
    {
        var userManager = new TestIdentityUserManager();
        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var httpContext = new DefaultHttpContext();
        var authService = new TestAuthenticationService();
        httpContext.RequestServices = new ServiceCollection()
            .AddSingleton<IAuthenticationService>(authService)
            .BuildServiceProvider();

        var service = BuildService(userManager, signInManager, currentTenant, tenantSettingsCache, httpContext);

        await service.SignInHttpContext(Guid.NewGuid(), "user@example.com", "Google");

        Assert.Equal(200, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task PerformTokenFederation_ValidLoginInfo_SignsInUser()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var signInManager = new TestSignInManager(userManager, new ClaimsPrincipal(new ClaimsIdentity()));
        var currentTenant = new TestCurrentTenant();
        var tenantSettingsCache = BuildTenantSettingsCacheService(currentTenant, TenantStatus.Active);
        var httpContext = new DefaultHttpContext();
        var authService = new TestAuthenticationService();
        httpContext.RequestServices = new ServiceCollection()
            .AddSingleton<IAuthenticationService>(authService)
            .BuildServiceProvider();

        var service = BuildService(userManager, signInManager, currentTenant, tenantSettingsCache, httpContext);
        var loginInfo = BuildExternalLoginInfo("user@example.com", "Google");
        var additionalClaims = new List<Claim> { new Claim("custom", "value") };

        await service.PerformTokenFederation(loginInfo, additionalClaims);

        Assert.NotEmpty(signInManager.CapturedClaims);
        Assert.Contains(signInManager.CapturedClaims, c => c.Type == "custom" && c.Value == "value");
    }
}
