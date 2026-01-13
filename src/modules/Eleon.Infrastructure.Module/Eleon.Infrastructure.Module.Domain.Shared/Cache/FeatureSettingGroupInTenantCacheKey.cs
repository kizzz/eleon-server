using System;

namespace VPortal.Core.Infrastructure.Module
{
  public class FeatureSettingGroupInTenantCacheKey
  {
    public string Group { get; set; }
    public Guid? TenantId { get; set; }
    public string Key { get; set; }
    public FeatureSettingGroupInTenantCacheKey(string group, Guid? tenantId, string key)
    {
      Group = group;
      TenantId = tenantId;
      Key = key;
    }

    //Builds the cache key
    public override string ToString()
    {
      return $"{Group}_{TenantId}_{Key}";
    }
  }
}
