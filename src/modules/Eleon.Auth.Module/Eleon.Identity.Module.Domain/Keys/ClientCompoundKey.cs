using System;

namespace Common.Module.Keys
{
  public class ClientCompoundKey : CompoundKey
  {
    protected static string HostTenantName = "host";

    public ClientCompoundKey(string clientKey, Guid? tenantId)
    {
      ClientKey = clientKey;
      TenantId = tenantId;
    }

    public string ClientKey { get; init; }
    public Guid? TenantId { get; init; }

    public static ClientCompoundKey Parse(string rawCompoundKey)
    {
      string[] parts = rawCompoundKey.Split(KeyDelimeter);
      if (parts.Length == 2)
      {
        string clientKey = parts[0];
        Guid? tenantId = parts[1] == HostTenantName ? null : Guid.Parse(parts[1]);
        return new ClientCompoundKey(clientKey, tenantId);
      }

      throw new ArgumentException("Could not parse the provided key.", nameof(rawCompoundKey));
    }

    public override string Key => CombineKeyParts(ClientKey, TenantId == null ? HostTenantName : TenantId.ToString());
  }
}
