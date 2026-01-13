using System;

namespace VPortal.Storage.Module.Cache
{
  public class ProviderInTenantCacheKey
  {
    public ProviderInTenantCacheKey(Guid? tenantId, string providerKey)
    {
      TenantId = tenantId;
      ProviderKey = providerKey;
    }

    public Guid? TenantId { get; }
    public string ProviderKey { get; }

    public override string ToString()
    {
      return $"{TenantId};{ProviderKey}";
    }
  }
}
