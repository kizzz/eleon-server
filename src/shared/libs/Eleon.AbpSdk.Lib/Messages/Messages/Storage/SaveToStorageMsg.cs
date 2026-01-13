using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.Storage;

[DistributedEvent]
public class SaveToStorageMsg : VportalEvent
{
  public string SettingsGroup { get; set; }
  public string BlobName { get; set; }
  public string Base64Data { get; set; }
  public bool OverwriteIfExists { get; set; } = false;
}

[DistributedEvent]
public class GetFromStorageMsg : VportalEvent
{
  public string SettingsGroup { get; set; }
  public string BlobName { get; set; }
}

public class SaveToStorageResponseMsg : VportalEvent
{
  public bool Success { get; set; }
}

public class GetFromStorageResponseMsg : VportalEvent
{
  public bool Success { get; set; }
  public string Base64Data { get; set; }
}
