using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using VPortal.Controllers;
using VPortal.Identity.Module.DomainServices;
using Xunit;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.HttpApi;

public class TokenControllerTests
{
    private static CustomTokenService BuildTokenService(out TestIdentityUserManager userManager)
    {
        userManager = new TestIdentityUserManager();
        var user = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", user.Id.ToString()) }, "test"));
        var signInManager = new TestSignInManager(userManager, principal);

        var client = new Client
        {
            ClientId = "VPortal_App",
            AllowedScopes = { "api" },
            AccessTokenLifetime = 3600
        };

        var clientStore = Substitute.For<IClientStore>();
        clientStore.FindClientByIdAsync("VPortal_App").Returns(Task.FromResult(client));

        var resourceStore = TestResourceStore.CreateDefault();

        var tokenService = new TestTokenService();
        var refreshTokenService = new TestRefreshTokenService();

        return new CustomTokenService(
            tokenService,
            refreshTokenService,
            clientStore,
            resourceStore,
            userManager,
            signInManager,
            new ConfigurationBuilder().Build());
    }

    [Fact]
    public async Task GetByUserIdAsync_Disabled_Throws()
    {
        var tokenService = BuildTokenService(out _);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            { "DebugSettings:EnableTestToken", "false" }
        }).Build();

        var controller = new TokenController(tokenService, config);

        await Assert.ThrowsAsync<Exception>(() => controller.GetByUserIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByUserIdAsync_Enabled_ReturnsToken()
    {
        var tokenService = BuildTokenService(out var userManager);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            { "DebugSettings:EnableTestToken", "true" }
        }).Build();

        var controller = new TokenController(tokenService, config);

        var user = await userManager.FindByNameAsync("user");
        var result = await controller.GetByUserIdAsync(user?.Id ?? Guid.Empty);

        Assert.Equal("jwt-token", result.AccessToken);
        Assert.Equal("Bearer", result.TokenType);
    }

    [Fact]
    public async Task GetByUserNameAsync_Disabled_Throws()
    {
        var tokenService = BuildTokenService(out _);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            { "DebugSettings:EnableTestToken", "false" }
        }).Build();

        var controller = new TokenController(tokenService, config);

        await Assert.ThrowsAsync<Exception>(() => controller.GetByUserNameAsync("user"));
    }

    [Fact]
    public async Task GetByUserIdAsync_UserNotFound_ThrowsException()
    {
        var tokenService = BuildTokenService(out _);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            { "DebugSettings:EnableTestToken", "true" }
        }).Build();

        var controller = new TokenController(tokenService, config);

        await Assert.ThrowsAsync<Exception>(() => controller.GetByUserIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByUserNameAsync_UserNotFound_ThrowsException()
    {
        var tokenService = BuildTokenService(out _);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            { "DebugSettings:EnableTestToken", "true" }
        }).Build();

        var controller = new TokenController(tokenService, config);

        await Assert.ThrowsAsync<Exception>(() => controller.GetByUserNameAsync("nonexistent"));
    }

    [Fact]
    public async Task GetByUserNameAsync_Enabled_ReturnsToken()
    {
        var tokenService = BuildTokenService(out _);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            { "DebugSettings:EnableTestToken", "true" }
        }).Build();

        var controller = new TokenController(tokenService, config);

        var result = await controller.GetByUserNameAsync("user");

        Assert.Equal("jwt-token", result.AccessToken);
        Assert.Equal("Bearer", result.TokenType);
    }
}
