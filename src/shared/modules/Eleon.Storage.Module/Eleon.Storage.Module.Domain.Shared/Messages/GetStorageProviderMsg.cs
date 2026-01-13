using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;

[DistributedEvent]
public class GetStorageProviderMsg : VportalEvent
{
  public string? StorageProviderId { get; set; }
}
