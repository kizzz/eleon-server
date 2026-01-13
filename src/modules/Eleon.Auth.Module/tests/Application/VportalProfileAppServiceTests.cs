using System;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using VPortal.Overrides.Account;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Application;

public class VportalProfileAppServiceTests : AppServiceTestBase
{
    private VportalProfileAppService BuildService(TestIdentityUserManager userManager = null, ICurrentUser currentUser = null)
    {
        userManager ??= new TestIdentityUserManager();
        currentUser ??= CreateMockCurrentUser(Guid.NewGuid(), "user");
        currentUser.IsAuthenticated.Returns(true);
        var identityOptions = CreateTestAbpDynamicOptionsManager<IdentityOptions>();

        var service = new VportalProfileAppService(
            userManager,
            identityOptions);
        var objectMapper = new TestObjectMapper();
        SetAppServiceDependencies(service, objectMapper, currentUser, CreateTestSettingProvider());
        return service;
    }

    [Fact]
    public async Task GetAsync_ReturnsProfileDto()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        userManager.SetGetByIdResult(user);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        var service = BuildService(userManager, currentUser);

        var result = await service.GetAsync();

        Assert.NotNull(result);
        Assert.Equal("user", result.UserName);
    }

    [Fact]
    public async Task UpdateAsync_ValidInput_UpdatesProfile()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        userManager.SetGetByIdResult(user);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        var service = BuildService(userManager, currentUser);

        var input = new UpdateProfileDto
        {
            UserName = "updateduser",
            Email = "updated@example.com",
            Name = "Updated",
            Surname = "User"
        };

        var result = await service.UpdateAsync(input);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_ValidInput_ChangesPassword()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);
        userManager.SetGetByIdResult(user);
        userManager.SetPasswordCheckResult(true);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        var service = BuildService(userManager, currentUser);

        var input = new ChangePasswordInput
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!"
        };

        await service.ChangePasswordAsync(input);
    }

    [Fact]
    public async Task ChangePasswordAsync_InvalidOldPassword_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        typeof(IdentityUser).GetProperty("PasswordHash")?.SetValue(user, "hash");
        userManager.AddUser(user);
        userManager.SetGetByIdResult(user);
        userManager.SetPasswordCheckResult(false);

        var currentUser = CreateMockCurrentUser(user.Id, "user");
        var service = BuildService(userManager, currentUser);

        var input = new ChangePasswordInput
        {
            CurrentPassword = "WrongPassword",
            NewPassword = "NewPassword123!"
        };

        await Assert.ThrowsAnyAsync<Exception>(() => service.ChangePasswordAsync(input));
    }
}
