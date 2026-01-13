using System;

namespace Common.Module.Keys
{
  public class ClientMachineCompoundKey : ClientCompoundKey
  {
    public ClientMachineCompoundKey(string clientKey, string machineKey, Guid? tenantId)
        : base(clientKey, tenantId)
    {
      MachineKey = machineKey;
    }

    public string MachineKey { get; init; }

    public new static ClientMachineCompoundKey Parse(string rawCompoundKey)
    {
      string[] parts = rawCompoundKey.Split(KeyDelimeter);
      if (parts.Length == 3)
      {
        string machineKey = parts[0];
        string clientKey = parts[1];
        Guid? tenantId = parts[2] == HostTenantName ? null : Guid.Parse(parts[2]);
        return new ClientMachineCompoundKey(clientKey, machineKey, tenantId);
      }

      throw new ArgumentException("Could not parse the provided key.", nameof(rawCompoundKey));
    }

    public override string Key => CombineKeyParts(MachineKey, base.Key);
  }
}
