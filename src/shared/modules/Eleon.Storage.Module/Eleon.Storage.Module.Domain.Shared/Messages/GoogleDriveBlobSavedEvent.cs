using Common.Module.Events;
using Messaging.Module.Messages;
using System;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider
{
  /// <summary>
  /// Published when a blob is successfully saved to Google Drive storage provider.
  /// Contains the Google Drive file ID and web view URL.
  /// </summary>
  [DistributedEvent]
  public class GoogleDriveBlobSavedEvent : VportalEvent
  {
    public string ContainerName { get; set; }
    public string BlobName { get; set; }
    public string FileId { get; set; }
    public string WebUrl { get; set; }
  }
}
