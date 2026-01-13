using Common.Module.Constants;
using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Accounting.Module.Entities
{
  [Audited]
  public class BillingInformationEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    // Company details
    public virtual string CompanyName { get; set; }
    public virtual string CompanyCID { get; set; }

    // Address fields
    public virtual string BillingAddressLine1 { get; set; }
    public virtual string BillingAddressLine2 { get; set; }
    public virtual string City { get; set; }
    public virtual string StateOrProvince { get; set; }
    public virtual string PostalCode { get; set; }
    public virtual string Country { get; set; }

    // Contact person details
    public virtual string ContactPersonName { get; set; }
    public virtual string ContactPersonEmail { get; set; }
    public virtual string ContactPersonTelephone { get; set; }

    // Payment method details
    public virtual PaymentMethod PaymentMethod { get; set; }

    private BillingInformationEntity() { }
    public BillingInformationEntity(Guid id)
        : base(id) { }
  }
}
