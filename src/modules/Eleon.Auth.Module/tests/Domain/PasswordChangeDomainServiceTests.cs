using System;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using VPortal.Identity.Module.DomainServices;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class PasswordChangeDomainServiceTests : DomainTestBase
{
    private PasswordChangeDomainService BuildService(
        TestIdentityUserManager userManager,
        ICurrentUser currentUser = null)
    {
        var logger = CreateMockLogger<PasswordChangeDomainService>();
        currentUser ??= CreateMockCurrentUser(Guid.NewGuid(), "user");

        return new PasswordChangeDomainService(
            logger,
            userManager,
            currentUser);
    }

    [Fact]
    public async Task ChangePassword_Unauthorized_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        var currentUser = CreateMockCurrentUser(null, null);

        var service = BuildService(userManager, currentUser);
        var result = await service.ChangePassword("OldPassword123!", "NewPassword123!");

        Assert.False(result);
    }

    [Fact]
    public async Task ChangePassword_UserNotFound_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var currentUser = CreateMockCurrentUser(Guid.NewGuid(), "user");

        var service = BuildService(userManager, currentUser);
        var result = await service.ChangePassword("OldPassword123!", "NewPassword123!");

        Assert.False(result);
    }

    [Fact]
    public async Task ChangePassword_InvalidOldPassword_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        userManager.SetPasswordCheckResult(false);

        var service = BuildService(userManager, currentUser);
        var result = await service.ChangePassword("WrongPassword", "NewPassword123!");

        Assert.False(result);
    }

    [Fact]
    public async Task ChangePassword_ValidPasswords_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        userManager.SetPasswordCheckResult(true);

        var service = BuildService(userManager, currentUser);
        var result = await service.ChangePassword("OldPassword123!", "NewPassword123!");

        Assert.True(result);
    }

    [Fact]
    public async Task ChangePassword_ValidPasswords_RemovesOldPassword()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        userManager.SetPasswordCheckResult(true);

        var service = BuildService(userManager, currentUser);
        var result = await service.ChangePassword("OldPassword123!", "NewPassword123!");

        Assert.True(result);
        Assert.True(userManager.RemovePasswordCalled);
    }

    [Fact]
    public async Task ChangePassword_ValidPasswords_AddsNewPassword()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        userManager.SetPasswordCheckResult(true);

        var service = BuildService(userManager, currentUser);
        var result = await service.ChangePassword("OldPassword123!", "NewPassword123!");

        Assert.True(result);
        Assert.True(userManager.AddPasswordCalled);
        Assert.Equal("NewPassword123!", userManager.LastPasswordAdded);
    }

    [Fact]
    public async Task ShouldChangePassword_UserNotFound_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var currentUser = CreateMockCurrentUser(Guid.NewGuid(), "user");

        var service = BuildService(userManager, currentUser);
        var result = await service.ShouldChangePassword(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task ShouldChangePassword_ShouldChangeOnNextLogin_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        typeof(IdentityUser).GetProperty("ShouldChangePasswordOnNextLogin")?.SetValue(user, true);
        userManager.AddUser(user);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        userManager.SetShouldPeriodicallyChangePassword(false);

        var service = BuildService(userManager, currentUser);
        var result = await service.ShouldChangePassword(user.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task ShouldChangePassword_PeriodicChangeRequired_ReturnsTrue()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        typeof(IdentityUser).GetProperty("ShouldChangePasswordOnNextLogin")?.SetValue(user, false);
        userManager.AddUser(user);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        userManager.SetShouldPeriodicallyChangePassword(true);

        var service = BuildService(userManager, currentUser);
        var result = await service.ShouldChangePassword(user.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task ShouldChangePassword_NoChangeRequired_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        typeof(IdentityUser).GetProperty("ShouldChangePasswordOnNextLogin")?.SetValue(user, false);
        userManager.AddUser(user);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        userManager.SetShouldPeriodicallyChangePassword(false);

        var service = BuildService(userManager, currentUser);
        var result = await service.ShouldChangePassword(user.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task ChangePassword_ExceptionThrown_ReturnsFalse()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        userManager.SetPasswordCheckResult(true);
        userManager.SetExceptionOnRemovePassword(new Exception("Database error"));

        var service = BuildService(userManager, currentUser);
        var result = await service.ChangePassword("OldPassword123!", "NewPassword123!");

        Assert.False(result);
    }
}

