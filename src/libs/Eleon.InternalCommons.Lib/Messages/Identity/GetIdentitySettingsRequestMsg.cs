using EleonsoftModuleCollector.Commons.Module.Constants.IdentitySettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Commons.Module.Messages.Identity;
public class GetIdentitySettingsRequestMsg
{
}

public class IdentitySettingsResponseMsg
{
  public List<IdentitySetting> Settings { get; set; }
}

public class IdentitySetting
{
  public string Name { get; set; }
  public string GroupName { get; set; }
  public string DisplayName { get; set; }
  public string Description { get; set; }
  public IdentitySettingType Type { get; set; }
  public string Value { get; set; }
}
