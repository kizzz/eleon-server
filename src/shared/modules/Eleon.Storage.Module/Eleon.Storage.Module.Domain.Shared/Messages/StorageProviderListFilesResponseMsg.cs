using Common.Module.Events;
using Messaging.Module.Messages;
using SharedModule.modules.Blob.Module.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.AbpSdk.Lib.modules.Messages.SystemMessages.StorageProvider
{
  public class StorageProviderListFilesResponseMsg
  {
    public List<VfsFileInfo> Files { get; set; } = new();
  }
}
