using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.Blob.Module;

//[DistributedEvent]
public class StorageProviderSettingsMsg
{
  public Guid StorageProviderId { get; set; }
}
