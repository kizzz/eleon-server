using Common.Module.Constants;
using Common.Module.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace VPortal.TenantManagement.Module.TenantHostname
{
  public class HostnameBindingConfiguration
  {
    public string Domain { get; set; }
    public string Subdomain { get; set; }
    public int Port { get; set; }
    public bool IsSsl { get; set; }
    public bool ResolveTenantBySubdomain { get; set; }

    public string GetSubdomain(string tenant, bool isSecure)
    {
      string separator = !tenant.IsNullOrEmpty() && !Subdomain.IsNullOrEmpty() ? "-" : string.Empty;
      string subdomain = $"{tenant?.ToLower() ?? string.Empty}{separator}{Subdomain}";

      if (isSecure)
      {
        subdomain = "secure" + (subdomain.NonEmpty() ? "-" : string.Empty) + subdomain;
      }

      return subdomain;
    }

    public static List<HostnameBindingConfiguration> Parse(IConfigurationSection cfg) => cfg.Get<List<HostnameBindingConfiguration>>();
  }
}
