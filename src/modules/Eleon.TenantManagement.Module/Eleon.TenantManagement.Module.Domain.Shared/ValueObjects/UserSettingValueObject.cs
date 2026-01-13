using Common.Module.Constants;
using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.ValueObjects
{
  public class UserSettingValueObject
  {
    public Dictionary<string, string> Settings { get; set; }
    //public BuCompanyEntity Company { get; set; }
    public List<string> AllowedModules { get; set; }
    public List<string> Errors { get; set; }
    public bool IsValidSetting { get; set; }
  }
}
