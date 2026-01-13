using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.RedirectUri;

[DistributedEvent]
public class ValidateRedirectUriResponseMsg
{
  public bool IsValid { get; set; }
}
