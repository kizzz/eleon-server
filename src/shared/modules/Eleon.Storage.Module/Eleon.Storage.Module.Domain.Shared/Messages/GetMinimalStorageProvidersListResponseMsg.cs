using Messaging.Module.Messages;
using SharedModule.modules.Blob.Module.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Messages;
public class GetMinimalStorageProvidersListResponseMsg : VportalEvent
{
  public bool IsSuccess { get; set; }
  public List<MinimalStorageProviderDto> StorageProviders { get; set; } = new();
}
