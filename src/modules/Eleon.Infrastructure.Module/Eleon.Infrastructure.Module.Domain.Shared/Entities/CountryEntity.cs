using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Infrastructure.Module.Entities
{
  public class CountryEntity : AggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }

    private CountryEntity() { }

    public CountryEntity(Guid id) : base(id)
    {
    }
  }
}
