using Common.Module.Constants;
using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Accounting.Module.Entities
{
  [Audited]
  public class ReceiptEntity : FullAuditedEntity<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual Guid InvoiceEntityId { get; set; }
    public virtual DateTime PaymentDate { get; set; }
    public virtual decimal Amount { get; set; }
    // New properties
    public virtual PaymentType PaymentType { get; set; }
    public virtual string Transaction { get; set; } // This could be a transaction ID or a more complex type

    // One-to-one navigation property to InvoiceEntity
    public virtual InvoiceEntity Invoice { get; set; }

    public ReceiptEntity() { }

    public ReceiptEntity(Guid id)
        : base(id) { }
  }
}
