using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Accounting.Module.Entities
{
  [Audited]
  public class InvoiceRowEntity : FullAuditedEntity<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual Guid InvoiceEntityId { get; set; }
    public virtual int Count { get; set; }
    public virtual decimal Price { get; set; }
    public virtual string Description { get; set; }

    public InvoiceRowEntity() { }

    public InvoiceRowEntity(Guid id)
        : base(id) { }

    #region Display properties

    [NotMapped] // Calculated property, not stored in the database
    public virtual decimal RowTotal => Count * Price;

    #endregion
  }
}
