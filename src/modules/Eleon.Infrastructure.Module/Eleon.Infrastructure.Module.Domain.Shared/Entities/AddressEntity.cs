using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Infrastructure.Module.Entities
{
  public class AddressEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public string EntityUid { get; set; }
    public string EntityName { get; set; }
    public string ParentUid { get; set; }
    public string AddressName { get; set; }
    public string CardCode { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string ObjType { get; set; }
    public string Building { get; set; }
    public string AddresType { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string AddrType { get; set; }
    public string StreetNo { get; set; }
    public string AddressHashCode { get; set; }

    private AddressEntity() { }

    public AddressEntity(Guid id) : base(id)
    {
    }
  }
}
