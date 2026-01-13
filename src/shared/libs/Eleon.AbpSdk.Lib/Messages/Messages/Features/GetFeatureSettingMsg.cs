using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.Features;

[DistributedEvent]
public class GetFeatureSettingMsg : VportalEvent
{
  public string Key { get; set; }
  public string Group { get; set; }
}

[DistributedEvent]
public class GetFeatureSettingResponseMsg : VportalEvent
{
  public string Value { get; set; }
  public string Type { get; set; }
  public bool IsEncrypted { get; set; }
  public bool IsRequired { get; set; }
}