using Common.Module.Events;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.Identity;

[DistributedEvent]
public class ValidApiKeyReponseMsg
{
  public IdentityApiKeyEto ApiKey { get; set; }
}
