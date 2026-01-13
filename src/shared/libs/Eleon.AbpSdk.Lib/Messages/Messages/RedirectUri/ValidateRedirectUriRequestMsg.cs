using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.RedirectUri;

[DistributedEvent]
public class ValidateRedirectUriRequestMsg
{
  public string RedirectUri { get; set; }
  public bool IsSignOut { get; set; }
}
