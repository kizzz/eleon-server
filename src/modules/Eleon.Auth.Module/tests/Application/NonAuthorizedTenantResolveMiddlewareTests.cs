using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using ExternalLogin.Module;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NSubstitute;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Application;

public class NonAuthorizedTenantResolveMiddlewareTests
{
    private sealed class TestMultiTenancyMiddleware : MultiTenancyMiddleware
    {
        public bool Invoked { get; private set; }

        public TestMultiTenancyMiddleware(ITenantConfigurationProvider tenantConfigurationProvider,
            ICurrentTenant currentTenant,
            IOptions<AbpAspNetCoreMultiTenancyOptions> options,
            ITenantResolveResultAccessor tenantResolveResultAccessor)
            : base(tenantConfigurationProvider, currentTenant, options, tenantResolveResultAccessor)
        {
        }

        public override Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Invoked = true;
            return next(context);
        }
    }

    private sealed class StubAuthHandler : IAuthenticationHandler
    {
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) => Task.CompletedTask;
        public Task<AuthenticateResult> AuthenticateAsync() => Task.FromResult(AuthenticateResult.NoResult());
        public Task ChallengeAsync(AuthenticationProperties properties) => Task.CompletedTask;
        public Task ForbidAsync(AuthenticationProperties properties) => Task.CompletedTask;
    }

    private sealed class TestExternalLoginOptionsConfigurator : IExternalLoginOptionsConfigurator
    {
        private readonly string _privateKey;

        public TestExternalLoginOptionsConfigurator(string privateKey)
        {
            _privateKey = privateKey;
        }

        public void ConfigureOptions(string authenticationSchemeName, Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions options)
        {
            options.ClientSecret = _privateKey;
        }
    }

    [Fact]
    public async Task InvokeAsync_Unauthenticated_ResolvesTenantFromState()
    {
        var tenantId = Guid.NewGuid();
        var currentTenant = new TestCurrentTenant();
        var dataProtectionProvider = DataProtectionProvider.Create("oidc-tests");
        var resolver = new OpenIdConnectStateResolver(dataProtectionProvider, NullLogger<OpenIdConnectStateResolver>.Instance);
        var privateKey = "private-key";

        string state;
        using (currentTenant.Change(tenantId, "tenant"))
        {
            var message = new OpenIdConnectMessage();
            var stateContext = new DefaultHttpContext();
            var stateServices = new ServiceCollection()
                .AddSingleton<ICurrentTenant>(currentTenant)
                .AddSingleton<IExternalLoginOptionsConfigurator>(new TestExternalLoginOptionsConfigurator(privateKey))
                .BuildServiceProvider();
            stateContext.RequestServices = stateServices;
            resolver.WriteOidcStateToOidcParameters(stateContext, message, "oidc", privateKey);
            state = message.GetParameter("state") as string;
        }

        var authService = new TestAuthenticationService
        {
            AuthenticateResult = AuthenticateResult.NoResult()
        };

        var schemeProvider = Substitute.For<IAuthenticationSchemeProvider>();
        schemeProvider.GetDefaultAuthenticateSchemeAsync()
            .Returns(Task.FromResult(new AuthenticationScheme("Test", "Test", typeof(StubAuthHandler))));

        var services = new ServiceCollection()
            .AddSingleton<IAuthenticationService>(authService)
            .AddSingleton<ICurrentTenant>(currentTenant)
            .AddSingleton<IExternalLoginOptionsConfigurator>(new TestExternalLoginOptionsConfigurator(privateKey))
            .BuildServiceProvider();

        var context = new DefaultHttpContext
        {
            RequestServices = services
        };
        context.Request.Headers["Cookie"] = $".oidc.state={Uri.EscapeDataString(state)}";

        var multiTenancyMiddleware = new TestMultiTenancyMiddleware(
            Substitute.For<ITenantConfigurationProvider>(),
            currentTenant,
            Options.Create(new AbpAspNetCoreMultiTenancyOptions()),
            Substitute.For<ITenantResolveResultAccessor>());

        var middleware = new NonAuthorizedTenantResolveMiddleware(
            schemeProvider,
            currentTenant,
            Options.Create(new AbpAspNetCoreMultiTenancyOptions()),
            resolver,
            multiTenancyMiddleware);

        var nextInvoked = false;
        await middleware.InvokeAsync(context, _ =>
        {
            nextInvoked = true;
            return Task.CompletedTask;
        });

        Assert.True(multiTenancyMiddleware.Invoked);
        Assert.True(nextInvoked);
        Assert.Equal(tenantId, currentTenant.Id);
    }

    [Fact]
    public async Task InvokeAsync_Authenticated_SkipsTenantResolution()
    {
        var currentTenant = new TestCurrentTenant();
        var authService = new TestAuthenticationService
        {
            AuthenticateResult = AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity("test")), "Test"))
        };

        var schemeProvider = Substitute.For<IAuthenticationSchemeProvider>();
        schemeProvider.GetDefaultAuthenticateSchemeAsync()
            .Returns(Task.FromResult(new AuthenticationScheme("Test", "Test", typeof(StubAuthHandler))));

        var services = new ServiceCollection()
            .AddSingleton<IAuthenticationService>(authService)
            .AddSingleton<ICurrentTenant>(currentTenant)
            .AddSingleton<IExternalLoginOptionsConfigurator>(new TestExternalLoginOptionsConfigurator("key"))
            .BuildServiceProvider();

        var context = new DefaultHttpContext
        {
            RequestServices = services
        };

        var dataProtectionProvider = DataProtectionProvider.Create("oidc-tests");
        var resolver = new OpenIdConnectStateResolver(dataProtectionProvider, NullLogger<OpenIdConnectStateResolver>.Instance);

        var multiTenancyMiddleware = new TestMultiTenancyMiddleware(
            Substitute.For<ITenantConfigurationProvider>(),
            currentTenant,
            Options.Create(new AbpAspNetCoreMultiTenancyOptions()),
            Substitute.For<ITenantResolveResultAccessor>());

        var middleware = new NonAuthorizedTenantResolveMiddleware(
            schemeProvider,
            currentTenant,
            Options.Create(new AbpAspNetCoreMultiTenancyOptions()),
            resolver,
            multiTenancyMiddleware);

        var nextInvoked = false;
        await middleware.InvokeAsync(context, _ =>
        {
            nextInvoked = true;
            return Task.CompletedTask;
        });

        Assert.False(multiTenancyMiddleware.Invoked);
        Assert.True(nextInvoked);
        Assert.Null(currentTenant.Id);
    }
}
