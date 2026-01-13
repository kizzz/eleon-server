using SharedModule.modules.Blob.Module.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
public class GetStorageProviderResponseMsg
{
  public StorageProviderDto StorageProvider { get; set; }
}
