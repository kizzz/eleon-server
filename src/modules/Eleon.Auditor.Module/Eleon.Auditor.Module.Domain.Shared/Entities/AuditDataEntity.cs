using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.Auditor.Module.Entities
{
  /// <summary>
  /// The entity contains the data that represents some state of a document.
  /// </summary>
  public class AuditDataEntity : CreationAuditedAggregateRoot<Guid>
  {
    /// <summary>
    /// Gets or sets the document data that should contain all the information needed to restore some specific version of a document.
    /// </summary>
    public virtual string DocumentData { get; set; }

    protected AuditDataEntity() { }

    public AuditDataEntity(Guid id)
    {
      Id = id;
    }
  }
}
