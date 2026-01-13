using Common.EventBus.Module;
using EleonS3.Domain.Shared;
using EleonS3.Domain.Shared.Objects;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.StorageProvider.Module;
using Logging.Module;
using SharedModule.modules.Blob.Module;
using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Shared;
using SharedModule.modules.Blob.Module.VfsShared;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Text.Json;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using VPortal.Storage.Module.Cache;

namespace EleonS3.Domain;

public class ObjectDomainService : DomainService
{
    private readonly SigV4Manager _sigV4Manager;
    private readonly IDistributedEventBus _eventBus;
    private readonly VfsStorageProviderCacheManager _vfsStorageProviderCacheService;
    private static readonly Dictionary<string, string> _tenantTelemetrySettingsCache = [];

    public ObjectDomainService(
        VfsStorageProviderCacheManager vfsStorageProviderCacheService,
        SigV4Manager sigV4Manager,
        IDistributedEventBus eventBus
        )
    {
        _sigV4Manager = sigV4Manager;
        _vfsStorageProviderCacheService = vfsStorageProviderCacheService;
        _eventBus = eventBus;
    }

    public async Task SaveAsync(string storageProviderId, string key, byte[] bytes)
    {

        var vfsProvider = await GetBlobProviderAsync(storageProviderId);

        await vfsProvider.SaveAsync(new VfsSaveArgs(storageProviderId, key, new MemoryStream(bytes), true));

    }

    public async Task<S3Object> GetAsync(string storageProviderId, string key, string range)
    {
        var response = new S3Object();
        response.IsFullObject = true;
        var vfsProvider = await GetBlobProviderAsync(storageProviderId);
        var bytes = await vfsProvider.GetAllBytesOrNullAsync(new VfsGetArgs(storageProviderId, key));

        if (bytes == null)
        {
            throw new Exception($"Object {key} not found in bucket {storageProviderId}");
        }

        if (!string.IsNullOrEmpty(range) && range.StartsWith("bytes="))
        {
            var parts = range["bytes=".Length..].Split('-', 2);
            if (long.TryParse(parts[0], out var start))
            {
                long end = bytes.Length - 1;
                if (parts.Length > 1 && long.TryParse(parts[1], out var parsedEnd))
                    end = parsedEnd;

                var slice = bytes.Skip((int)start).Take((int)(end - start + 1)).ToArray();
                response.End = end;
                response.Start = start;
                response.Slice = slice;
                response.IsFullObject = false;
            }
        }
        response.Content = bytes;
        return response;
    }

    public async Task<S3Object> GetHeadAsync(string storageProviderId, string key)
    {

        var vfsProvider = await GetBlobProviderAsync(storageProviderId);
        var bytes = await vfsProvider.GetAllBytesOrNullAsync(new VfsGetArgs(storageProviderId, key));
        if (bytes == null)
        {
            return null;
        }
        var storageKey = $"{storageProviderId}/{key}";

        var s3Object = new S3Object();
        s3Object.Content = bytes;
        s3Object.ETag = "\"" + Guid.NewGuid().ToString("N") + "\"";

        return s3Object;
    }

    public async Task<bool> DeleteAsync(string storageProviderId, string key)
    {
        var vfsProvider = await GetBlobProviderAsync(storageProviderId);
        await vfsProvider.DeleteAsync(new VfsDeleteArgs(storageProviderId, key));
        return true;
    }

    public async Task<string> ListFileInfosAsync(string storageProviderId, string? prefix)
    {
        var vfsProvider = await GetBlobProviderAsync(storageProviderId);
        var objs = await vfsProvider.ListAsync(new VfsListArgs(storageProviderId, prefix ?? "./", true));

        var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.000Z");
        var sb = new StringBuilder();

        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<ListBucketResult xmlns=\"http://s3.amazonaws.com/doc/2006-03-01/\">");
        sb.AppendLine($"  <Name>{storageProviderId}</Name>");
        sb.AppendLine($"  <Prefix>{System.Security.SecurityElement.Escape(prefix)}</Prefix>");
        sb.AppendLine("  <Delimiter></Delimiter>");
        sb.AppendLine($"  <KeyCount>{objs.Count}</KeyCount>");
        sb.AppendLine("  <MaxKeys>1000</MaxKeys>");
        sb.AppendLine("  <IsTruncated>false</IsTruncated>");

        foreach (var o in objs)
        {
            sb.AppendLine("  <Contents>");
            sb.AppendLine($"    <Key>{System.Security.SecurityElement.Escape(o.Key)}</Key>");
            sb.AppendLine($"    <LastModified>{now}</LastModified>");
            sb.AppendLine($"    <ETag>\"{Guid.NewGuid():N}\"</ETag>");
            sb.AppendLine($"    <Size>{o.Size}</Size>");
            sb.AppendLine("    <StorageClass>STANDARD</StorageClass>");
            sb.AppendLine("  </Contents>");
        }

        sb.AppendLine("</ListBucketResult>");

        return sb.ToString();
    }

    private async Task<IVfsBlobProvider> GetBlobProviderAsync(string storageProviderId)
    {
        Guid? tenantId = CurrentTenant.Id;
        string? realStorageProviderId = storageProviderId;

        bool isSigV4 = _sigV4Manager.SigV4AuthorizationSucceded;

        // Resolve tenantId and realStorageProviderId
        if (isSigV4)
        {
            if (!Guid.TryParse(storageProviderId, out var parsedTenantId))
            {
                throw new Exception($"Tenant ID '{storageProviderId}' is not valid.");
            }

            tenantId = parsedTenantId;
            realStorageProviderId = _vfsStorageProviderCacheService.GetCachedTelemetryProviderId(tenantId);
        }
        else
        {

            if (realStorageProviderId == BlobMessagingConsts.TelemetryStorageProviderKey)
            {
                realStorageProviderId = _vfsStorageProviderCacheService.GetCachedTelemetryProviderId(tenantId);
            }
        }

        // Execute the whole resolution under the SigV4 tenant context (like your original, but broader scope)
        if (isSigV4)
        {
            using (CurrentTenant.Change(tenantId))
            {
                return await _vfsStorageProviderCacheService.ResolveProviderAsync(tenantId, realStorageProviderId);
            }
        }

        // Non-SigV4 path (no tenant scope change, matching original behavior)
        return await _vfsStorageProviderCacheService.ResolveProviderAsync(tenantId, realStorageProviderId);
    }
}
