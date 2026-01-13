using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.EventBus.Module;
using Eleon.TestsBase.Lib.TestBase;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonS3.Domain;
using EleonS3.Domain.Shared.Objects;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.VfsShared;
using EleonsoftSdk.modules.StorageProvider.Module;
using Logging.Module;
using NSubstitute;
using SharedModule.modules.Blob.Module;
using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Shared;
using SharedModule.modules.Blob.Module.VfsShared;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace EleonS3.Test.Tests;

public class ObjectDomainServiceTests : MockingTestBase
{
    [Fact]
    public async Task GetAsync_WithRange_ReturnsSlice()
    {
        var tenantId = Guid.NewGuid();
        var providerId = "provider";
        var (service, provider) = BuildService(tenantId, providerId, sigV4: false);

        await service.SaveAsync(providerId, "key", new byte[] { 1, 2, 3, 4, 5 });

        var result = await service.GetAsync(providerId, "key", "bytes=1-3");

        Assert.False(result.IsFullObject);
        Assert.Equal(1, result.Start);
        Assert.Equal(3, result.End);
        Assert.Equal(new byte[] { 2, 3, 4 }, result.Slice);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, result.Content);
        Assert.True(provider.Stored.ContainsKey("key"));
    }

    [Fact]
    public async Task GetAsync_RangeWithoutEnd_UsesLastByte()
    {
        var tenantId = Guid.NewGuid();
        var providerId = "provider";
        var (service, _) = BuildService(tenantId, providerId, sigV4: false);

        await service.SaveAsync(providerId, "key", new byte[] { 10, 20, 30, 40 });

        var result = await service.GetAsync(providerId, "key", "bytes=2-");

        Assert.False(result.IsFullObject);
        Assert.Equal(2, result.Start);
        Assert.Equal(3, result.End);
        Assert.Equal(new byte[] { 30, 40 }, result.Slice);
    }

    [Fact]
    public async Task GetAsync_InvalidRange_ReturnsFullObject()
    {
        var tenantId = Guid.NewGuid();
        var providerId = "provider";
        var (service, _) = BuildService(tenantId, providerId, sigV4: false);

        await service.SaveAsync(providerId, "key", new byte[] { 1, 2, 3 });

        var result = await service.GetAsync(providerId, "key", "bytes=abc-2");

        Assert.True(result.IsFullObject);
        Assert.Equal(new byte[] { 1, 2, 3 }, result.Content);
        Assert.Null(result.Slice);
    }

    [Fact]
    public async Task GetAsync_WhenMissing_Throws()
    {
        var (service, _) = BuildService(Guid.NewGuid(), "provider", sigV4: false);

        await Assert.ThrowsAsync<Exception>(() => service.GetAsync("provider", "missing", null));
    }

    [Fact]
    public async Task GetHeadAsync_WhenMissing_ReturnsNull()
    {
        var (service, _) = BuildService(Guid.NewGuid(), "provider", sigV4: false);

        var result = await service.GetHeadAsync("provider", "missing");

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesObject()
    {
        var (service, provider) = BuildService(Guid.NewGuid(), "provider", sigV4: false);
        await service.SaveAsync("provider", "key", new byte[] { 9 });

        var ok = await service.DeleteAsync("provider", "key");

        Assert.True(ok);
        Assert.False(provider.Stored.ContainsKey("key"));
    }

    [Fact]
    public async Task ListFileInfosAsync_ReturnsXml()
    {
        var (service, provider) = BuildService(Guid.NewGuid(), "provider", sigV4: false);
        provider.Stored["prefix/a.txt"] = new byte[] { 1 };
        provider.Stored["prefix/b.txt"] = new byte[] { 2, 3 };

        var xml = await service.ListFileInfosAsync("provider", "prefix");

        Assert.Contains("<Name>provider</Name>", xml);
        Assert.Contains("<KeyCount>2</KeyCount>", xml);
        Assert.Contains("<Key>prefix/a.txt</Key>", xml);
        Assert.Contains("<Key>prefix/b.txt</Key>", xml);
    }

    [Fact]
    public async Task ListFileInfosAsync_NullPrefix_ReturnsXml()
    {
        var (service, provider) = BuildService(Guid.NewGuid(), "provider", sigV4: false);
        provider.Stored["./a.txt"] = new byte[] { 1 };

        var xml = await service.ListFileInfosAsync("provider", null);

        Assert.Contains("<Name>provider</Name>", xml);
        Assert.Contains("<Prefix></Prefix>", xml);
        Assert.Contains("<KeyCount>1</KeyCount>", xml);
        Assert.Contains("<Key>./a.txt</Key>", xml);
    }

    [Fact]
    public async Task SaveAsync_TelemetryKey_UsesCachedProvider()
    {
        var tenantId = Guid.NewGuid();
        var providerId = "telemetry-provider";
        var (service, provider) = BuildService(tenantId, providerId, sigV4: false);

        await service.SaveAsync(BlobMessagingConsts.TelemetryStorageProviderKey, "key", new byte[] { 7 });

        Assert.True(provider.Stored.ContainsKey("key"));
    }

    [Fact]
    public async Task SaveAsync_SigV4_InvalidTenantId_Throws()
    {
        var (service, _) = BuildService(Guid.NewGuid(), "provider", sigV4: true);

        await Assert.ThrowsAsync<Exception>(() => service.SaveAsync("not-a-guid", "key", new byte[] { 1 }));
    }

    [Fact]
    public async Task GetAsync_SigV4_UsesTenantFromPath()
    {
        var tenantId = Guid.NewGuid();
        var providerId = "provider";
        var (service, provider) = BuildService(tenantId, providerId, sigV4: true);

        await service.SaveAsync(tenantId.ToString(), "key", new byte[] { 5, 6 });

        var result = await service.GetAsync(tenantId.ToString(), "key", null);

        Assert.True(result.IsFullObject);
        Assert.Equal(new byte[] { 5, 6 }, result.Content);
        Assert.True(provider.Stored.ContainsKey("key"));
    }

    private (ObjectDomainService Service, FakeVfsBlobProvider Provider) BuildService(Guid tenantId, string providerId, bool sigV4)
    {
        var provider = new FakeVfsBlobProvider();
        var cacheManager = BuildCacheManager(tenantId, providerId, provider);
        var sigManager = new SigV4Manager { SigV4AuthorizationSucceded = sigV4 };
        var eventBus = Substitute.For<IDistributedEventBus>();
        var service = new ObjectDomainService(cacheManager, sigManager, eventBus);

        SetCurrentTenant(service, TestMockHelpers.CreateMockCurrentTenant(tenantId, "tenant"));
        return (service, provider);
    }

    private VfsStorageProviderCacheManager BuildCacheManager(Guid tenantId, string providerId, IVfsBlobProvider provider)
    {
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<VfsStorageProviderCacheManager>>();
        var eventBus = Substitute.For<IDistributedEventBus>();
        var cacheService = new VfsBlobProviderCacheService();
        var manager = new VfsStorageProviderCacheManager(logger, eventBus, cacheService);

        manager.CacheProviderMapping(tenantId, providerId);
        cacheService.Register(BuildCacheKey(tenantId, providerId), provider);
        return manager;
    }

    private static string BuildCacheKey(Guid? tenantId, string storageProviderId)
        => $"{tenantId};{storageProviderId}";

    private static void SetCurrentTenant(ObjectDomainService service, ICurrentTenant currentTenant)
    {
        var lazyServiceProvider = Substitute.For<IAbpLazyServiceProvider>();
        lazyServiceProvider.LazyGetRequiredService<ICurrentTenant>().Returns(currentTenant);

        var prop = typeof(Volo.Abp.Domain.Services.DomainService)
            .GetProperty("LazyServiceProvider", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        prop?.SetValue(service, lazyServiceProvider);
    }

    private sealed class FakeVfsBlobProvider : IVfsBlobProvider
    {
        public Dictionary<string, byte[]> Stored { get; } = new(StringComparer.OrdinalIgnoreCase);

        public Task<IReadOnlyList<VfsFileInfo>> ListAsync(VfsListArgs args)
        {
            var prefix = args.BlobName ?? string.Empty;
            var result = Stored.Where(x => x.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(x => new VfsFileInfo { Key = x.Key, Size = x.Value.Length, IsFolder = false })
                .ToList();
            return Task.FromResult<IReadOnlyList<VfsFileInfo>>(result);
        }

        public async Task SaveAsync(VfsSaveArgs args)
        {
            using var ms = new MemoryStream();
            await args.BlobStream.CopyToAsync(ms);
            Stored[args.BlobName] = ms.ToArray();
        }

        public Task<bool> DeleteAsync(VfsDeleteArgs args)
        {
            Stored.Remove(args.BlobName);
            return Task.FromResult(true);
        }

        public Task<Stream?> GetOrNullAsync(VfsGetArgs args)
        {
            if (!Stored.TryGetValue(args.BlobName, out var data))
            {
                return Task.FromResult<Stream?>(null);
            }

            return Task.FromResult<Stream?>(new MemoryStream(data));
        }

        public Task<byte[]?> GetAllBytesOrNullAsync(VfsGetArgs args)
        {
            return Task.FromResult(Stored.TryGetValue(args.BlobName, out var data) ? data : null);
        }

        public Task<bool> ExistsAsync(VfsExistArgs args) => Task.FromResult(Stored.ContainsKey(args.BlobName));

        public Task<bool> TestAsync(VfsTestArgs args) => Task.FromResult(true);

        public Task<VfsPagedResult> ListPagedAsync(VfsListPagedArgs args) => throw new NotImplementedException();

        public void Dispose() { }
    }
}
