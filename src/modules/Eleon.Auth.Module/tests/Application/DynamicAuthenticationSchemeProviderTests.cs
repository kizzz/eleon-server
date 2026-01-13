using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using ExternalLogin.Module;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSubstitute;
using VPortal.Identity.Module.AuthenticationSchemes;
using Xunit;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Application;

public class DynamicAuthenticationSchemeProviderTests : DomainTestBase
{
    private DynamicAuthenticationSchemeProvider BuildService(IAuthenticationSchemeStore store)
    {
        var options = Options.Create(new AuthenticationOptions());
        return new DynamicAuthenticationSchemeProvider(options, store);
    }

    [Fact]
    public async Task GetSchemeAsync_ExistingScheme_ReturnsScheme()
    {
        var store = Substitute.For<IAuthenticationSchemeStore>();
        var scheme = new AuthenticationScheme("Google", "Google", typeof(TestAuthenticationHandler));
        store.GetAuthenticationSchemes().Returns(new ValueTask<IEnumerable<AuthenticationScheme>>(new[] { scheme }));

        var service = BuildService(store);
        var result = await service.GetSchemeAsync("Google");

        Assert.NotNull(result);
        Assert.Equal("Google", result.Name);
    }

    [Fact]
    public async Task GetSchemeAsync_NonExistentScheme_ReturnsNull()
    {
        var store = Substitute.For<IAuthenticationSchemeStore>();
        store.GetAuthenticationSchemes().Returns(new ValueTask<IEnumerable<AuthenticationScheme>>(Array.Empty<AuthenticationScheme>()));

        var options = Options.Create(new AuthenticationOptions());
        var service = new DynamicAuthenticationSchemeProvider(options, store);
        var result = await service.GetSchemeAsync("NonExistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllSchemesAsync_ReturnsAllSchemes()
    {
        var store = Substitute.For<IAuthenticationSchemeStore>();
        var dynamicScheme = new AuthenticationScheme("Google", "Google", typeof(TestAuthenticationHandler));
        store.GetAuthenticationSchemes().Returns(new ValueTask<IEnumerable<AuthenticationScheme>>(new[] { dynamicScheme }));

        var options = Options.Create(new AuthenticationOptions());
        var service = new DynamicAuthenticationSchemeProvider(options, store);
        var result = await service.GetAllSchemesAsync();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetRequestHandlerSchemesAsync_ReturnsHandlerSchemes()
    {
        var store = Substitute.For<IAuthenticationSchemeStore>();
        var dynamicScheme = new AuthenticationScheme("Google", "Google", typeof(TestAuthenticationHandler));
        store.GetAuthenticationSchemes().Returns(new ValueTask<IEnumerable<AuthenticationScheme>>(new[] { dynamicScheme }));

        var options = Options.Create(new AuthenticationOptions());
        var service = new DynamicAuthenticationSchemeProvider(options, store);
        var result = await service.GetRequestHandlerSchemesAsync();

        Assert.NotNull(result);
    }

    private class TestAuthenticationHandler : IAuthenticationHandler
    {
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        public Task ChallengeAsync(AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            return Task.CompletedTask;
        }
    }
}

