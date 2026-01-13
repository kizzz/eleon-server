using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Extensions
{
  public static class CurrentTenantExtensions
  {
    public static IDisposable ChangeDefault(this ICurrentTenant tenant)
    {
      throw new Exception("Deprecated");
    }
  }
}
