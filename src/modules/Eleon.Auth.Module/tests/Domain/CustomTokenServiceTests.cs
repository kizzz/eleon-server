using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Volo.Abp.Identity;
using Xunit;
using VPortal.Identity.Module.DomainServices;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class CustomTokenServiceTests : DomainTestBase
{
    private static CustomTokenService BuildService(
        TestIdentityUserManager userManager,
        VPortal.Identity.Module.SignIn.SignInManager signInManager,
        ITokenService tokenService = null,
        IRefreshTokenService refreshTokenService = null,
        IClientStore clientStore = null,
        IResourceStore resourceStore = null)
    {
        tokenService ??= new TestTokenService();
        refreshTokenService ??= new TestRefreshTokenService();
        clientStore ??= Substitute.For<IClientStore>();
        resourceStore ??= TestResourceStore.CreateDefault();

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
    public async Task CreateTokensByUserIdAsync_UserNotFound_Throws()
    {
        var userManager = new TestIdentityUserManager();
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var signInManager = new TestSignInManager(userManager, principal);
        var service = BuildService(userManager, signInManager);

        await Assert.ThrowsAsync<Exception>(() => service.CreateTokensByUserIdAsync("client", Guid.NewGuid().ToString()));
    }

    [Fact]
    public async Task CreateTokensByUserIdAsync_ClientNotFound_Throws()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var signInManager = new TestSignInManager(userManager, principal);

        var clientStore = Substitute.For<IClientStore>();
        clientStore.FindClientByIdAsync(Arg.Any<string>()).Returns(Task.FromResult<Client>(null));

        var service = BuildService(userManager, signInManager, clientStore: clientStore);

        await Assert.ThrowsAsync<Exception>(() => service.CreateTokensByUserIdAsync("missing", user.Id.ToString()));
    }

    [Fact]
    public async Task CreateTokensByUserIdAsync_ReturnsTokenResponse()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var claimsIdentity = new ClaimsIdentity(new[] { new Claim("sub", user.Id.ToString()) }, "test");
        var principal = new ClaimsPrincipal(claimsIdentity);
        var signInManager = new TestSignInManager(userManager, principal);

        var client = new Client
        {
            ClientId = "client",
            AllowedScopes = { "api" },
            AccessTokenLifetime = 3600
        };

        var clientStore = Substitute.For<IClientStore>();
        clientStore.FindClientByIdAsync("client").Returns(Task.FromResult(client));

        var resourceStore = TestResourceStore.CreateDefault();

        var tokenService = new TestTokenService();
        var refreshTokenService = new TestRefreshTokenService();

        var service = BuildService(userManager, signInManager, tokenService, refreshTokenService, clientStore, resourceStore);

        var result = await service.CreateTokensByUserIdAsync("client", user.Id.ToString());

        Assert.Equal("jwt-token", result.AccessToken);
        Assert.Equal("Bearer", result.TokenType);
        Assert.Equal(3600, result.ExpiresIn);
        Assert.Equal("refresh", result.RefreshToken);
        var capturedRequest = tokenService.LastAccessTokenRequest;
        Assert.NotNull(capturedRequest);
        Assert.Contains(capturedRequest.Subject.Claims, c => c.Type == "idp");
        Assert.Contains(capturedRequest.Subject.Claims, c => c.Type == "auth_time");
    }

    [Fact]
    public async Task CreateTokensByUserNameAsync_ReturnsTokenResponse()
    {
        var userManager = new TestIdentityUserManager();
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        userManager.AddUser(user);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", user.Id.ToString()) }, "test"));
        var signInManager = new TestSignInManager(userManager, principal);

        var client = new Client
        {
            ClientId = "client",
            AllowedScopes = { "api" },
            AccessTokenLifetime = 1200
        };

        var clientStore = Substitute.For<IClientStore>();
        clientStore.FindClientByIdAsync("client").Returns(Task.FromResult(client));

        var resourceStore = TestResourceStore.CreateDefault();

        var tokenService = new TestTokenService();
        var refreshTokenService = new TestRefreshTokenService();

        var service = BuildService(userManager, signInManager, tokenService, refreshTokenService, clientStore, resourceStore);

        var result = await service.CreateTokensByUserNameAsync("client", "user");

        Assert.Equal("jwt-token", result.AccessToken);
        Assert.Equal(1200, result.ExpiresIn);
    }
}
