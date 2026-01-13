using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Messages;
public class GetMinimalStorageProvidersListMsg : VportalEvent
{
  public List<Guid> ProviderIds { get; set; }
}
