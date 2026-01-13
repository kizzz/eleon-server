using Common.Module.Constants;
using Common.Module.Helpers;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SaveLightweightStorageItemMsg : VportalEvent
  {
    public string Key { get; set; }
    public string BlobBase64 { get; set; }
    public SizeUnits? MaxSizeUnit { get; set; }
    public long? MaxSize { get; set; }
    public TimeSpan? CacheExpiration { get; set; }
    public List<string> RequiredPermissions { get; set; }
  }
}
