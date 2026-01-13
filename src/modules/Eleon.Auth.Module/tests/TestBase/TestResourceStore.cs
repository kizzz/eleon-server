using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;

public sealed class TestResourceStore(Resources resources) : IResourceStore
{
    private readonly Resources _resources = resources ?? throw new ArgumentNullException(nameof(resources));

    public static TestResourceStore CreateDefault()
    {
        var resources = new Resources(
            Array.Empty<IdentityResource>(),
            new[] { new ApiResource("api", "api") },
            new[] { new ApiScope("api") });
        return new TestResourceStore(resources);
    }

    public Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
    {
        return Task.FromResult<IEnumerable<ApiResource>>(_resources.ApiResources);
    }

    public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        return Task.FromResult<IEnumerable<ApiResource>>(_resources.ApiResources);
    }

    public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
    {
        return Task.FromResult<IEnumerable<ApiScope>>(_resources.ApiScopes);
    }

    public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        return Task.FromResult<IEnumerable<IdentityResource>>(_resources.IdentityResources);
    }

    public Task<Resources> FindResourcesByScopeAsync(IEnumerable<string> scopeNames)
    {
        return Task.FromResult(_resources);
    }

    public Task<Resources> GetAllResourcesAsync()
    {
        return Task.FromResult(_resources);
    }
}
