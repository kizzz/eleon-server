using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.AbpSdk.Lib.modules.Messages.SystemMessages.StorageProvider
{
  [DistributedEvent]
  public class StorageProviderDeleteFilesMsg : VportalEvent
  {
    public Guid StorageProviderId { get; set; }
    public List<string> Keys { get; set; }
  }
}
