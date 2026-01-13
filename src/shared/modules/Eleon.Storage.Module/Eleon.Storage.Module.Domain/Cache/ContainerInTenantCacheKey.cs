using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VPortal.Storage.Module.Cache
{
  public class ContainerInTenantCacheKey
  {
    public ContainerInTenantCacheKey(Guid? tenantId, string providerKey, string settingsVersionHash = null)
    {
      TenantId = tenantId;
      ProviderKey = providerKey;
      SettingsVersionHash = settingsVersionHash ?? string.Empty;
    }

    public Guid? TenantId { get; }
    public string ProviderKey { get; }
    public string SettingsVersionHash { get; }

    public override string ToString()
    {
      return $"{TenantId};{ProviderKey};{SettingsVersionHash}";
    }

    public override bool Equals(object obj)
    {
      return obj is ContainerInTenantCacheKey other &&
             TenantId == other.TenantId &&
             ProviderKey == other.ProviderKey &&
             SettingsVersionHash == other.SettingsVersionHash;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(TenantId, ProviderKey, SettingsVersionHash);
    }

    /// <summary>
    /// Computes a hash from a settings dictionary to use as version identifier.
    /// Hash is computed from sorted key-value pairs to ensure consistency.
    /// </summary>
    public static string ComputeSettingsHash(Dictionary<string, string> settings)
    {
      if (settings == null || settings.Count == 0)
      {
        return string.Empty;
      }

      // Sort by key for consistent hashing
      var sortedPairs = settings.OrderBy(kvp => kvp.Key);
      var hashInput = string.Join("|", sortedPairs.Select(kvp => $"{kvp.Key}={kvp.Value ?? string.Empty}"));

      using (var sha256 = SHA256.Create())
      {
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
        // Use first 16 bytes as hex string for reasonable key length
        return BitConverter.ToString(hashBytes, 0, 16).Replace("-", string.Empty);
      }
    }
  }
}
