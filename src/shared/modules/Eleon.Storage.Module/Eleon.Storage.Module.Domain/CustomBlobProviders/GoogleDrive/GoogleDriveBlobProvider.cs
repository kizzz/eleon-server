using Common.EventBus.Module;
using Commons.Module.Messages.Google;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Storage.Module.BlobProviders.GoogleDrive;

public class GoogleDriveBlobProvider : BlobProviderBase, ITransientDependency
{
  private readonly IDistributedEventBus _eventBus;

  public GoogleDriveBlobProvider(
      IDistributedEventBus eventBus)
  {
    _eventBus = eventBus;
  }

  public override async Task SaveAsync(BlobProviderSaveArgs args)
  {
    // 1) Upload
    var uploadBytes = await ReadAllAsync(args.BlobStream);
    var uploadResp = await _eventBus.RequestAsync<GoogleDriveUploadResponseMsg>(
        new GoogleDriveUploadRequestMsg
        {
          Name = args.BlobName,                                 // keep original name
          Content = uploadBytes,                                // file content
          ContentType = await GetContentType(export: false),    // body content-type
          MimeType = await GetContentType(export: true)         // target Drive MIME (Google type)
        });

    if (!string.IsNullOrWhiteSpace(uploadResp.Error) || string.IsNullOrWhiteSpace(uploadResp.FileId))
      throw new IOException(uploadResp.Error ?? "Upload failed: missing file id");

    var fileId = uploadResp.FileId;

    // 2) Create public (view) link
    var linkResp = await _eventBus.RequestAsync<GoogleDriveCreateLinkResponseMsg>(
        new GoogleDriveCreateLinkRequestMsg
        {
          FileId = fileId,
          PermissionRole = "reader"
        });

    if (!string.IsNullOrWhiteSpace(linkResp.Error))
      throw new IOException(linkResp.Error);

    // Publish event with file metadata instead of using singleton cache
    await _eventBus.PublishAsync(new GoogleDriveBlobSavedEvent
    {
      ContainerName = args.ContainerName,
      BlobName = args.BlobName,
      FileId = fileId,
      WebUrl = linkResp.WebViewLink ?? string.Empty
    });
  }

  public override async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
  {
    var resp = await _eventBus.RequestAsync<GoogleDriveDeleteResponseMsg>(
        new GoogleDriveDeleteRequestMsg { FileId = args.BlobName });

    if (!string.IsNullOrWhiteSpace(resp.Error))
      throw new IOException(resp.Error);

    return resp.Success;
  }

  public override Task<bool> ExistsAsync(BlobProviderExistsArgs args)
  {
    // Optional: implement a HEAD/Exists message if needed.
    return Task.FromResult(true);
  }

  public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
  {
    var resp = await _eventBus.RequestAsync<GoogleDriveDownloadResponseMsg>(
        new GoogleDriveDownloadRequestMsg
        {
          FileId = args.BlobName,
          ContentType = await GetContentType(export: true), // export to .docx in your mapping
          FileName = args.BlobName
        });

    if (!string.IsNullOrWhiteSpace(resp.Error))
      throw new IOException(resp.Error);

    return resp.Content is null
        ? Stream.Null
        : new MemoryStream(resp.Content, writable: false);
  }

  private static async Task<byte[]> ReadAllAsync(Stream input)
  {
    if (input is MemoryStream ms && ms.TryGetBuffer(out var seg))
      return seg.ToArray();

    using var buffer = new MemoryStream();
    await input.CopyToAsync(buffer);
    return buffer.ToArray();
  }

  private Task<string> GetContentType(bool export = false)
  {
    // Keep your original mapping:
    // export=true  -> target/export MIME (e.g., .docx)
    // export=false -> upload body content type
    if (export)
      return Task.FromResult("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
    return Task.FromResult("application/vnd.google-apps.document");
  }
}
