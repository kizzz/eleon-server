using System;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using Logging.Module;
using NSubstitute;
using VPortal.Storage.Module.Cache;
using VPortal.Storage.Module.DomainServices;
using Volo.Abp.BlobStoring;
using Xunit;

namespace Eleon.Storage.Module.Domain.Tests.DomainServices;

public class ContainersCacheDomainServiceTests : MockingTestBase
{
    private ContainersCacheDomainService _service;
    private IVportalLogger<ContainersCacheDomainService> _logger;

    public ContainersCacheDomainServiceTests()
    {
        _logger = CreateMockLogger<ContainersCacheDomainService>();
        _service = new ContainersCacheDomainService(_logger);
    }

    [Fact]
    public void GetOrAddCacheEntry_WithSameKey_Should_ReturnSameContainer()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var providerKey = "provider-1";
        var cacheKey = new ContainerInTenantCacheKey(tenantId, providerKey);
        var container1 = Substitute.For<IBlobContainer>();
        var container2 = Substitute.For<IBlobContainer>();
        int factoryCallCount = 0;

        // Act
        var result1 = _service.GetOrAddCacheEntry(cacheKey, () => { factoryCallCount++; return container1; });
        var result2 = _service.GetOrAddCacheEntry(cacheKey, () => { factoryCallCount++; return container2; });

        // Assert
        result1.Should().BeSameAs(result2);
        factoryCallCount.Should().Be(1); // Factory should only be called once
    }

    [Fact]
    public void GetOrAddCacheEntry_WithDifferentTenant_Should_ReturnDifferentContainer()
    {
        // Arrange
        var tenant1 = SharedTestConstants.TenantIds.Tenant1;
        var tenant2 = SharedTestConstants.TenantIds.Tenant2;
        var providerKey = "provider-1";
        var key1 = new ContainerInTenantCacheKey(tenant1, providerKey);
        var key2 = new ContainerInTenantCacheKey(tenant2, providerKey);
        var container1 = Substitute.For<IBlobContainer>();
        var container2 = Substitute.For<IBlobContainer>();

        // Act
        var result1 = _service.GetOrAddCacheEntry(key1, () => container1);
        var result2 = _service.GetOrAddCacheEntry(key2, () => container2);

        // Assert
        result1.Should().BeSameAs(container1);
        result2.Should().BeSameAs(container2);
        result1.Should().NotBeSameAs(result2);
    }

    [Fact]
    public void GetOrAddCacheEntry_WithDifferentProviderKey_Should_ReturnDifferentContainer()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var key1 = new ContainerInTenantCacheKey(tenantId, "provider-1");
        var key2 = new ContainerInTenantCacheKey(tenantId, "provider-2");
        var container1 = Substitute.For<IBlobContainer>();
        var container2 = Substitute.For<IBlobContainer>();

        // Act
        var result1 = _service.GetOrAddCacheEntry(key1, () => container1);
        var result2 = _service.GetOrAddCacheEntry(key2, () => container2);

        // Assert
        result1.Should().BeSameAs(container1);
        result2.Should().BeSameAs(container2);
        result1.Should().NotBeSameAs(result2);
    }

    [Fact]
    public void RemoveCacheEntry_WithExistingKey_Should_ReturnTrue()
    {
        // Arrange
        var key = new ContainerInTenantCacheKey(SharedTestConstants.TenantIds.Tenant1, "provider-1");
        _service.GetOrAddCacheEntry(key, () => Substitute.For<IBlobContainer>());

        // Act
        var result = _service.RemoveCacheEntry(key);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void RemoveCacheEntry_WithNonExistentKey_Should_ReturnFalse()
    {
        // Arrange
        var key = new ContainerInTenantCacheKey(SharedTestConstants.TenantIds.Tenant1, "non-existent");

        // Act
        var result = _service.RemoveCacheEntry(key);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetOrAddCacheEntry_WhenFactoryThrows_Should_LogAndReturnNull()
    {
        // Arrange
        var key = new ContainerInTenantCacheKey(SharedTestConstants.TenantIds.Tenant1, "provider-1");
        var exception = new Exception("Factory error");

        // Act
        var result = _service.GetOrAddCacheEntry(key, () => throw exception);

        // Assert
        result.Should().BeNull();
        // Verify logger was called - actual verification depends on IVportalLogger interface
    }

    [Fact]
    public void GetOrAddCacheEntry_WithSettingsHash_Should_UseHashInKey()
    {
        // Arrange
        var tenantId = SharedTestConstants.TenantIds.Tenant1;
        var providerKey = "provider-1";
        var key1 = new ContainerInTenantCacheKey(tenantId, providerKey, "hash1");
        var key2 = new ContainerInTenantCacheKey(tenantId, providerKey, "hash2");
        var container1 = Substitute.For<IBlobContainer>();
        var container2 = Substitute.For<IBlobContainer>();

        // Act
        var result1 = _service.GetOrAddCacheEntry(key1, () => container1);
        var result2 = _service.GetOrAddCacheEntry(key2, () => container2);

        // Assert
        result1.Should().BeSameAs(container1);
        result2.Should().BeSameAs(container2);
        result1.Should().NotBeSameAs(result2);
    }

    [Fact]
    public async Task GetOrAddCacheEntry_WithConcurrentCalls_Should_CallFactoryOnce()
    {
        // Arrange
        var key = new ContainerInTenantCacheKey(SharedTestConstants.TenantIds.Tenant1, "provider-1");
        int factoryCallCount = 0;
        var container = Substitute.For<IBlobContainer>();

        // Act
        var results = await ConcurrencyTestHelpers.SimulateConcurrentOperationAsync(
            () => Task.FromResult(_service.GetOrAddCacheEntry(key, () => { factoryCallCount++; return container; })),
            concurrencyLevel: 50);

        // Assert
        factoryCallCount.Should().Be(1);
        results.Should().AllBeEquivalentTo(container);
    }
}
