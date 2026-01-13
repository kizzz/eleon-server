using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Xunit;
using ExternalLogin.Module;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Application;

public class OpenIdConnectOptionsProviderTests
{
    private sealed class CountingOptionsFactory : IOptionsFactory<OpenIdConnectOptions>
    {
        public int CreateCalls { get; private set; }

        public OpenIdConnectOptions Create(string name)
        {
            CreateCalls++;
            return new OpenIdConnectOptions { ClientId = name };
        }
    }

    [Fact]
    public void Get_CachesPerTenantAndName()
    {
        var currentTenant = new TestCurrentTenant();
        var factory = new CountingOptionsFactory();
        var provider = new OpenIdConnectOptionsProvider(factory, currentTenant);

        var first = provider.Get("oidc");
        var second = provider.Get("oidc");

        Assert.Same(first, second);
        Assert.Equal(1, factory.CreateCalls);
    }

    [Fact]
    public void Get_DifferentTenant_CreatesNewInstance()
    {
        var currentTenant = new TestCurrentTenant();
        var factory = new CountingOptionsFactory();
        var provider = new OpenIdConnectOptionsProvider(factory, currentTenant);

        var host = provider.Get("oidc");
        using (currentTenant.Change(Guid.NewGuid(), "tenant"))
        {
            var tenant = provider.Get("oidc");
            Assert.NotSame(host, tenant);
        }

        Assert.Equal(2, factory.CreateCalls);
    }

    [Fact]
    public void ClearCache_ForcesNewOptions()
    {
        var currentTenant = new TestCurrentTenant();
        var factory = new CountingOptionsFactory();
        var provider = new OpenIdConnectOptionsProvider(factory, currentTenant);

        var first = provider.Get("oidc");
        provider.ClearCache();
        var second = provider.Get("oidc");

        Assert.NotSame(first, second);
        Assert.Equal(2, factory.CreateCalls);
    }

    [Fact]
    public async Task Get_ConcurrentAccess_ReturnsSameInstance()
    {
        var currentTenant = new TestCurrentTenant();
        var factory = new CountingOptionsFactory();
        var provider = new OpenIdConnectOptionsProvider(factory, currentTenant);

        var tasks = new Task<OpenIdConnectOptions>[5];
        var startGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        for (var i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                await startGate.Task;
                return provider.Get("oidc");
            });
        }

        startGate.SetResult(true);
        var all = Task.WhenAll(tasks);
        var completed = await Task.WhenAny(all, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.Same(all, completed);
        var results = await all;

        var first = results[0];
        foreach (var result in results)
        {
            Assert.Same(first, result);
        }
    }
}
