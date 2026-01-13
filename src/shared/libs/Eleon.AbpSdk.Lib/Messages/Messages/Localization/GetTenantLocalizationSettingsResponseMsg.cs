using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.Messages.Localization;

[DistributedEvent]
public class GetTenantLocalizationSettingsResponseMsg
{
  public string CultureName { get; set; }
  public string UiCultureName { get; set; }
}
