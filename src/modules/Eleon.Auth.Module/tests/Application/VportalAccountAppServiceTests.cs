using System;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Volo.Abp.Account;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Identity;
using VPortal.Overrides.Account;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Application;

public class VportalAccountAppServiceTests : AppServiceTestBase
{
    private VportalAccountAppService BuildService(TestIdentityUserManager userManager = null)
    {
        userManager ??= new TestIdentityUserManager();
        var accountEmailer = Substitute.For<IAccountEmailer>();
        var roleRepository = Substitute.For<IIdentityRoleRepository>();
        var identitySecurityLogManager = BuildIdentitySecurityLogManager(userManager);
        var identityOptions = CreateTestAbpDynamicOptionsManager<IdentityOptions>();
        var securityLogRepository = Substitute.For<IIdentitySecurityLogRepository>();

        var service = new VportalAccountAppService(
            userManager,
            accountEmailer,
            roleRepository,
            identitySecurityLogManager,
            identityOptions,
            securityLogRepository);
        var objectMapper = new TestObjectMapper();
        var currentUser = CreateMockCurrentUser(Guid.NewGuid(), "user");
        currentUser.IsAuthenticated.Returns(true);
        SetAppServiceDependencies(service, objectMapper, currentUser, CreateTestSettingProvider());
        return service;
    }

    [Fact]
    public async Task RegisterAsync_ValidInput_ReturnsUserDto()
    {
        var userManager = new TestIdentityUserManager();
        var service = BuildService(userManager);

        var input = new RegisterDto
        {
            UserName = "newuser",
            EmailAddress = "newuser@example.com",
            Password = "Password123!",
            AppName = "TestApp"
        };

        var result = await service.RegisterAsync(input);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task RegisterAsync_InvalidInput_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        var existingUser = new IdentityUser(Guid.NewGuid(), "existing", "existing@example.com");
        userManager.AddUser(existingUser);

        var service = BuildService(userManager);

        var input = new RegisterDto
        {
            UserName = "existing",
            EmailAddress = "existing@example.com",
            Password = "Password123!",
            AppName = "TestApp"
        };

        await Assert.ThrowsAnyAsync<Exception>(() => service.RegisterAsync(input));
    }

    [Fact]
    public async Task ResetPasswordAsync_ValidInput_ResetsPassword()
    {
        var userManager = new TestIdentityUserManager();
        var service = BuildService(userManager);

        var input = new ResetPasswordDto
        {
            UserId = Guid.NewGuid(),
            ResetToken = "valid-token",
            Password = "NewPassword123!"
        };

        await service.ResetPasswordAsync(input);
    }

    [Fact]
    public async Task ResetPasswordAsync_InvalidToken_ThrowsException()
    {
        var userManager = new TestIdentityUserManager();
        userManager.SetResetPasswordResult(IdentityResult.Failed());
        var service = BuildService(userManager);

        var input = new ResetPasswordDto
        {
            UserId = Guid.NewGuid(),
            ResetToken = "invalid-token",
            Password = "NewPassword123!"
        };

        await Assert.ThrowsAnyAsync<Exception>(() => service.ResetPasswordAsync(input));
    }
}
