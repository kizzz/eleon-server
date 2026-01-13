using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.MultiTenancy.Module;
public class EleonMultiTenancyOptions
{
  public bool Enabled { get; set; } = true;
  public bool SuppressUnresolvedTenant { get; set; } = false;

  // key value = (tenantId, domains)
  public Dictionary<string, string[]> TenantDomains { get; set; } = new Dictionary<string, string[]>();

  public const string DefaultSectionName = "MultiTenancySettings";
}
