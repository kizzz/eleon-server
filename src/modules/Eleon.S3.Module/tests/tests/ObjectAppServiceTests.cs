using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonS3.Application.Contracts.Objects;
using EleonS3.Application.Objects;
using EleonS3.Domain;
using EleonS3.Domain.Shared.Objects;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.VfsShared;
using EleonsoftSdk.modules.StorageProvider.Module;
using Logging.Module;
using NSubstitute;
using SharedModule.modules.Blob.Module;
using SharedModule.modules.Blob.Module.Shared;
using SharedModule.modules.Blob.Module.VfsShared;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace EleonS3.Test.Tests;

public class ObjectAppServiceTests
{
    [Fact]
    public async Task SaveAsync_DelegatesToDomain()
    {
        var (service, provider) = BuildService();

        await service.SaveAsync("provider", "key", new byte[] { 1, 2, 3 });

        Assert.True(provider.Stored.ContainsKey("key"));
    }

    [Fact]
    public async Task GetAsync_ReturnsDto()
    {
        var (service, _) = BuildService();

        var dto = await service.GetAsync("provider", "key", null);

        Assert.True(dto.IsFullObject);
        Assert.Equal(new byte[] { 1, 2, 3 }, dto.Content);
    }

    [Fact]
    public async Task GetHeadAsync_WhenMissing_ReturnsNull()
    {
        var (service, _) = BuildService(seedData: false);

        var dto = await service.GetHeadAsync("provider", "missing");

        Assert.Null(dto);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue()
    {
        var (service, provider) = BuildService();

        var result = await service.DeleteAsync("provider", "key");

        Assert.True(result);
        Assert.False(provider.Stored.ContainsKey("key"));
    }

    [Fact]
    public async Task GetListFileInfoAsync_ReturnsXml()
    {
        var (service, _) = BuildService();

        var result = await service.GetListFileInfoAsync("provider", "key");

        Assert.NotNull(result);
        Assert.Contains("<ListBucketResult", result.Xml);
    }

    private static (ObjectAppService Service, FakeVfsBlobProvider Provider) BuildService(bool seedData = true)
    {
        var tenantId = Guid.NewGuid();
        var providerId = "provider";
        var provider = new FakeVfsBlobProvider();
        if (seedData)
        {
            provider.Stored["key"] = new byte[] { 1, 2, 3 };
        }

        var domain = BuildDomainService(provider, tenantId, providerId);
        var service = new ObjectAppService(domain);

        var mapper = new TestObjectMapper();
        var currentUser = TestMockHelpers.CreateMockCurrentUser(Guid.NewGuid(), "user");
        ServiceProviderTestHelpers.SetAppServiceDependencies(service, mapper, currentUser);

        return (service, provider);
    }

    private static ObjectDomainService BuildDomainService(FakeVfsBlobProvider provider, Guid tenantId, string providerId)
    {
        var cache = BuildCacheManager(provider, tenantId, providerId);
        var sigManager = new SigV4Manager { SigV4AuthorizationSucceded = false };
        var eventBus = Substitute.For<IDistributedEventBus>();
        var service = new ObjectDomainService(cache, sigManager, eventBus);

        var currentTenant = TestMockHelpers.CreateMockCurrentTenant(tenantId, "tenant");
        var lazyServiceProvider = Substitute.For<Volo.Abp.DependencyInjection.IAbpLazyServiceProvider>();
        lazyServiceProvider.LazyGetRequiredService<ICurrentTenant>().Returns(currentTenant);

        typeof(Volo.Abp.Domain.Services.DomainService)
            .GetProperty("LazyServiceProvider", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            ?.SetValue(service, lazyServiceProvider);

        return service;
    }

    private static VfsStorageProviderCacheManager BuildCacheManager(IVfsBlobProvider provider, Guid tenantId, string providerId)
    {
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<VfsStorageProviderCacheManager>>();
        var eventBus = Substitute.For<IDistributedEventBus>();
        var cacheService = new VfsBlobProviderCacheService();
        var manager = new VfsStorageProviderCacheManager(logger, eventBus, cacheService);
        manager.CacheProviderMapping(tenantId, providerId);
        cacheService.Register($"{tenantId};{providerId}", provider);
        return manager;
    }

    private sealed class TestObjectMapper : IObjectMapper
    {
        public IAutoObjectMappingProvider AutoObjectMappingProvider => null;

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            if (source is S3Object s3Object && typeof(TDestination) == typeof(ObjectMetadataDto))
            {
                if (s3Object == null)
                {
                    return default;
                }
                var dto = new ObjectMetadataDto
                {
                    ContentLength = (int)(s3Object.Content?.LongLength ?? 0),
                    ETag = s3Object.ETag
                };
                return (TDestination)(object)dto;
            }

            return default;
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return destination;
        }
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
