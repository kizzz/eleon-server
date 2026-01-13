using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using ExternalLogin.Module;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Application;

public class OpenIdConnectStateResolverTests
{
    private const string AuthScheme = "oidc";
    private const string PrivateKey = "private-key";

    private sealed class TestExternalLoginOptionsConfigurator(TestCurrentTenant currentTenant, Dictionary<string, string> keys)
        : IExternalLoginOptionsConfigurator
    {
        public void ConfigureOptions(string authenticationSchemeName, Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions options)
        {
            var tenantKey = currentTenant.Id?.ToString() ?? "host";
            if (keys.TryGetValue(tenantKey, out var secret))
            {
                options.ClientSecret = secret;
            }
        }
    }

    private static DefaultHttpContext BuildHttpContext(IServiceProvider services, string stateQuery = null, string stateCookie = null)
    {
        var context = new DefaultHttpContext();
        context.RequestServices = services;
        if (!string.IsNullOrWhiteSpace(stateQuery))
        {
            context.Request.QueryString = new QueryString($"?state={Uri.EscapeDataString(stateQuery)}");
        }
        if (!string.IsNullOrWhiteSpace(stateCookie))
        {
            context.Request.Headers["Cookie"] = $".oidc.state={Uri.EscapeDataString(stateCookie)}";
        }
        return context;
    }

    private static OpenIdConnectStateResolver BuildResolver(IDataProtectionProvider provider)
    {
        return new OpenIdConnectStateResolver(provider, LoggerFactory.Create(builder => { }).CreateLogger<OpenIdConnectStateResolver>());
    }

    private static string ProtectState(IDataProtector dataProtector, JObject state)
    {
        return dataProtector.Protect(state.ToString(Newtonsoft.Json.Formatting.None));
    }

    private static string ComputeSignature(string salt, DateTime date, Guid? tenantId, string authScheme, string privateKey)
    {
        var inputString = $"{salt}{date}{tenantId}{authScheme}";
        var dataHash = ComputeSha256(inputString);
        return ComputeSha256(inputString + dataHash + privateKey);
    }

    private static string ComputeSha256(string input)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    [Fact]
    public void IsStatePresent_FalseWhenMissing()
    {
        var currentTenant = new TestCurrentTenant();
        var services = new ServiceCollection()
            .AddSingleton(currentTenant)
            .AddSingleton<ICurrentTenant>(currentTenant)
            .AddSingleton<IExternalLoginOptionsConfigurator>(new TestExternalLoginOptionsConfigurator(currentTenant, new()))
            .BuildServiceProvider();

        var resolver = BuildResolver(DataProtectionProvider.Create("tests"));
        var context = BuildHttpContext(services);

        Assert.False(resolver.IsStatePresent(context));
    }

    [Fact]
    public void IsStatePresent_TrueWhenStateExists()
    {
        var currentTenant = new TestCurrentTenant();
        Dictionary<string, string> keys = new() { ["host"] = PrivateKey };
        var services = new ServiceCollection()
            .AddSingleton(currentTenant)
            .AddSingleton<ICurrentTenant>(currentTenant)
            .AddSingleton<IExternalLoginOptionsConfigurator>(new TestExternalLoginOptionsConfigurator(currentTenant, keys))
            .BuildServiceProvider();

        var dataProtectionProvider = DataProtectionProvider.Create("tests");
        var resolver = BuildResolver(dataProtectionProvider);
        var message = new OpenIdConnectMessage();
        var context = BuildHttpContext(services);
        resolver.WriteOidcStateToOidcParameters(context, message, AuthScheme, PrivateKey);
        var encrypted = message.GetParameter("state") as string;
        var requestContext = BuildHttpContext(services, stateCookie: encrypted);

        Assert.True(resolver.IsStatePresent(requestContext));
    }

    [Fact]
    public void TrySetCurrentTenantFromOidcState_SetsTenantFromState()
    {
        var currentTenant = new TestCurrentTenant();
        Dictionary<string, string> keys = new();
        var services = new ServiceCollection()
            .AddSingleton(currentTenant)
            .AddSingleton<ICurrentTenant>(currentTenant)
            .AddSingleton<IExternalLoginOptionsConfigurator>(new TestExternalLoginOptionsConfigurator(currentTenant, keys))
            .BuildServiceProvider();

        var dataProtectionProvider = DataProtectionProvider.Create("tests");
        var resolver = BuildResolver(dataProtectionProvider);

        var tenantA = Guid.NewGuid();
        keys[tenantA.ToString()] = PrivateKey;

        using (currentTenant.Change(tenantA))
        {
            var message = new OpenIdConnectMessage();
            var context = BuildHttpContext(services);
            resolver.WriteOidcStateToOidcParameters(context, message, AuthScheme, PrivateKey);

            var encrypted = message.GetParameter("state") as string;
            using (currentTenant.Change(Guid.NewGuid()))
            {
                var requestContext = BuildHttpContext(services, stateCookie: encrypted);
                resolver.TrySetCurrentTenantFromOidcState(requestContext);
                Assert.Equal(tenantA, currentTenant.Id);
            }
        }
    }

    [Fact]
    public void TrySetCurrentTenantFromOidcState_TamperedState_Throws()
    {
        var currentTenant = new TestCurrentTenant();
        Dictionary<string, string> keys = new();
        var services = new ServiceCollection()
            .AddSingleton(currentTenant)
            .AddSingleton<ICurrentTenant>(currentTenant)
            .AddSingleton<IExternalLoginOptionsConfigurator>(new TestExternalLoginOptionsConfigurator(currentTenant, keys))
            .BuildServiceProvider();

        var dataProtectionProvider = DataProtectionProvider.Create("tests");
        var resolver = BuildResolver(dataProtectionProvider);

        var tenant = Guid.NewGuid();
        keys[tenant.ToString()] = PrivateKey;

        string encrypted;
        using (currentTenant.Change(tenant))
        {
            var message = new OpenIdConnectMessage();
            var context = BuildHttpContext(services);
            resolver.WriteOidcStateToOidcParameters(context, message, AuthScheme, PrivateKey);
            encrypted = message.GetParameter("state") as string;
        }

        var chars = encrypted.ToCharArray();
        chars[0] = chars[0] == 'A' ? 'B' : 'A';
        var tampered = new string(chars);
        var requestContext = BuildHttpContext(services, stateCookie: tampered);

        Assert.ThrowsAny<Exception>(() => resolver.TrySetCurrentTenantFromOidcState(requestContext));
    }

    [Fact]
    public void TrySetCurrentTenantFromOidcState_ExpiredState_Throws()
    {
        var currentTenant = new TestCurrentTenant();
        Dictionary<string, string> keys = new();
        var services = new ServiceCollection()
            .AddSingleton(currentTenant)
            .AddSingleton<ICurrentTenant>(currentTenant)
            .AddSingleton<IExternalLoginOptionsConfigurator>(new TestExternalLoginOptionsConfigurator(currentTenant, keys))
            .BuildServiceProvider();

        var dataProtectionProvider = DataProtectionProvider.Create("tests-expired");
        var resolver = BuildResolver(dataProtectionProvider);
        var dataProtector = dataProtectionProvider.CreateProtector("OpenIdStateResolver");

        var tenant = Guid.NewGuid();
        keys[tenant.ToString()] = PrivateKey;

        string encrypted;
        using (currentTenant.Change(tenant))
        {
            var message = new OpenIdConnectMessage();
            var context = BuildHttpContext(services);
            resolver.WriteOidcStateToOidcParameters(context, message, AuthScheme, PrivateKey);
            encrypted = message.GetParameter("state") as string;
        }

        var json = dataProtector.Unprotect(encrypted);
        var state = JObject.Parse(json);

        var expiredDate = DateTime.UtcNow.AddMinutes(-11);
        state["Date"] = expiredDate;

        var salt = state["Salt"]?.ToString();
        var tenantIdValue = state["TenantId"]?.ToObject<Guid?>();
        var authScheme = state["AuthScheme"]?.ToString();

        state["Signature"] = ComputeSignature(salt, expiredDate, tenantIdValue, authScheme, PrivateKey);

        var expiredEncrypted = ProtectState(dataProtector, state);
        var requestContext = BuildHttpContext(services, stateCookie: expiredEncrypted);

        var ex = Assert.ThrowsAny<Exception>(() => resolver.TrySetCurrentTenantFromOidcState(requestContext));
        Assert.Contains("expired", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
