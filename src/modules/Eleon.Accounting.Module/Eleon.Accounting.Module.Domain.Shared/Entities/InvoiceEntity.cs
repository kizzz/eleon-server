using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Accounting.Module.Entities
{
  [Audited]
  public class InvoiceEntity : FullAuditedEntity<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }

    // Foreign key for Account
    public virtual Guid AccountEntityId { get; set; }
    // Customer's billing information
    public virtual string CustomerName { get; set; }
    public virtual string CompanyCID { get; set; }
    public virtual string BillingAddress { get; set; }
    public virtual string BillingCity { get; set; }
    public virtual string BillingState { get; set; }
    public virtual string BillingCountry { get; set; }
    public virtual string BillingPostalCode { get; set; }

    // Common currency for all rows in the invoice
    public virtual string Currency { get; set; }

    // Navigation property to InvoiceRows
    public virtual List<InvoiceRowEntity> InvoiceRows { get; set; }

    // One-to-one relationship with ReceiptEntity
    public virtual ReceiptEntity Receipt { get; set; }

    private InvoiceEntity() { }

    public InvoiceEntity(Guid id) : base(id)
    {
      InvoiceRows = new List<InvoiceRowEntity>();
    }

    #region Display properties
    [NotMapped] // Calculated property, not stored in the database
    public virtual decimal Total
    {
      get { return InvoiceRows?.Sum(ir => ir.RowTotal) ?? 0; }
    }
    #endregion
  }
}
