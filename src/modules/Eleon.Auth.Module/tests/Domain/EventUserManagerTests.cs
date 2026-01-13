using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Volo.Abp.Caching;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Threading;
using VPortal.Identity.Module.DomainServices;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Logging.Module;
using Eleon.TestsBase.Lib.TestHelpers;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class EventUserManagerTests : DomainTestBase
{
    private EventUserManager BuildService(TestIdentityUserManager baseManager)
    {
        var roleRepo = Substitute.For<IIdentityRoleRepository>();
        var userRepo = Substitute.For<IIdentityUserRepository>();
        var store = new TestIdentityUserStore(userRepo, roleRepo, user => baseManager.GetRolesForUser(user));
        
        // Set up userRepo to return users from baseManager
        userRepo.FindAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(callInfo => 
            {
                var id = callInfo.Arg<Guid>();
                var user = baseManager.FindByIdAsync(id.ToString()).Result;
                return Task.FromResult(user);
            });
        userRepo.FindByNormalizedUserNameAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(callInfo =>
            {
                var userName = callInfo.Arg<string>();
                var user = baseManager.FindByNameAsync(userName).Result;
                return Task.FromResult(user);
            });
        userRepo.FindByNormalizedEmailAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(callInfo =>
            {
                var email = callInfo.Arg<string>();
                var user = baseManager.FindByEmailAsync(email).Result;
                return Task.FromResult(user);
            });
        userRepo.GetRolesAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                var user = baseManager.FindByIdAsync(id.ToString()).Result;
                var roles = user == null
                    ? new List<Volo.Abp.Identity.IdentityRole>()
                    : baseManager.GetRolesForUser(user)
                        .Select(roleName => new Volo.Abp.Identity.IdentityRole(Guid.NewGuid(), roleName))
                        .ToList();
                return Task.FromResult(roles);
            });
        userRepo.GetRoleNamesAsync(Arg.Any<Guid>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                var user = baseManager.FindByIdAsync(id.ToString()).Result;
                var roles = user == null ? new List<string>() : baseManager.GetRolesForUser(user).ToList();
                return Task.FromResult(roles);
            });
        var options = OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<IdentityOptions>();
        var passwordHasher = Substitute.For<IPasswordHasher<IdentityUser>>();
        passwordHasher.VerifyHashedPassword(Arg.Any<IdentityUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(_ => baseManager.GetPasswordCheckResult()
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed);
        var userValidators = new List<IUserValidator<IdentityUser>>();
        var passwordValidators = new List<IPasswordValidator<IdentityUser>>();
        var normalizer = Substitute.For<ILookupNormalizer>();
        normalizer.NormalizeName(Arg.Any<string>())
            .Returns(callInfo => callInfo.Arg<string>()?.ToUpperInvariant());
        normalizer.NormalizeEmail(Arg.Any<string>())
            .Returns(callInfo => callInfo.Arg<string>()?.ToUpperInvariant());
        var errors = new IdentityErrorDescriber();
        var services = Substitute.For<IServiceProvider>();
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<IdentityUserManager>>();
        var cancellationTokenProvider = Substitute.For<ICancellationTokenProvider>();
        var orgUnitRepo = Substitute.For<IOrganizationUnitRepository>();
        var settingProvider = Substitute.For<ISettingProvider>();
        var eventBus = CreateMockEventBus();
        var linkUserRepo = Substitute.For<IIdentityLinkUserRepository>();
        var cache = Substitute.For<IDistributedCache<AbpDynamicClaimCacheItem>>();
        var vportalLogger = CreateMockLogger<EventUserManager>();

        return new EventUserManager(
            store,
            roleRepo,
            userRepo,
            options,
            passwordHasher,
            userValidators,
            passwordValidators,
            normalizer,
            errors,
            services,
            logger,
            cancellationTokenProvider,
            orgUnitRepo,
            settingProvider,
            eventBus,
            linkUserRepo,
            cache,
            vportalLogger);
    }

    [Fact]
    public async Task FindByIdAsync_UserExists_ReturnsUser()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var service = BuildService(userManager);
        var result = await service.FindByIdAsync(user.Id.ToString());

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task FindByIdAsync_UserNotExists_ReturnsNull()
    {
        var userManager = new TestIdentityUserManager();
        var service = BuildService(userManager);
        var result = await service.FindByIdAsync(Guid.NewGuid().ToString());

        Assert.Null(result);
    }

    [Fact]
    public async Task FindByNameAsync_UserExists_ReturnsUser()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var service = BuildService(userManager);
        var result = await service.FindByNameAsync("user");

        Assert.NotNull(result);
        Assert.Equal("user", result.UserName);
    }

    [Fact]
    public async Task FindByEmailAsync_UserExists_ReturnsUser()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var service = BuildService(userManager);
        var result = await service.FindByEmailAsync("user@example.com");

        Assert.NotNull(result);
        Assert.Equal("user@example.com", result.Email);
    }

    [Fact]
    public async Task CheckPasswordAsync_ValidPassword_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        typeof(IdentityUser).GetProperty("PasswordHash")?.SetValue(user, "hash");
        userManager.AddUser(user);
        userManager.SetPasswordCheckResult(true);

        var service = BuildService(userManager);
        var result = await service.CheckPasswordAsync(user, "correctpassword");

        Assert.True(result);
    }

    [Fact]
    public async Task CheckPasswordAsync_InvalidPassword_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        typeof(IdentityUser).GetProperty("PasswordHash")?.SetValue(user, "hash");
        userManager.AddUser(user);
        userManager.SetPasswordCheckResult(false);

        var service = BuildService(userManager);
        var result = await service.CheckPasswordAsync(user, "wrongpassword");

        Assert.False(result);
    }

    [Fact]
    public async Task IsLockedOutAsync_LockedOut_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        typeof(IdentityUser).GetProperty("LockoutEnabled")?.SetValue(user, true);
        typeof(IdentityUser).GetProperty("LockoutEnd")?.SetValue(user, DateTimeOffset.UtcNow.AddMinutes(10));
        userManager.AddUser(user);

        var service = BuildService(userManager);
        var result = await service.IsLockedOutAsync(user);

        Assert.True(result);
    }

    [Fact]
    public async Task GetTwoFactorEnabledAsync_Enabled_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        typeof(IdentityUser).GetProperty("TwoFactorEnabled")?.SetValue(user, true);
        userManager.AddUser(user);

        var service = BuildService(userManager);
        var result = await service.GetTwoFactorEnabledAsync(user);

        Assert.True(result);
    }

    [Fact]
    public async Task GetClaimsAsync_ReturnsUserClaims()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var service = BuildService(userManager);
        var result = await service.GetClaimsAsync(user);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task IsInRoleAsync_UserInRole_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        await userManager.AddToRoleAsync(user, "Admin");

        var service = BuildService(userManager);
        var result = await service.IsInRoleAsync(user, "Admin");

        Assert.True(result);
    }
}
