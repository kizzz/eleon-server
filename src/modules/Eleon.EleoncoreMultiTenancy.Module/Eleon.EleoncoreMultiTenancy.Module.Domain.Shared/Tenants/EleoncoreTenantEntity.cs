using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.TenantManagement;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Entities
{
  public class EleoncoreTenantEntity : Tenant
  {
    private static readonly ILookupNormalizer _normalizer
= new UpperInvariantLookupNormalizer();   // built-in implementation

    public EleoncoreTenantEntity(Guid id, string name)
        : base(id, name, _normalizer.NormalizeName(name))
    { }
  }
}
