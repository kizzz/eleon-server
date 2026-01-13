using Common.Module.Constants;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.Auditor.Module.Entities
{
  /// <summary>
  /// An entity representing the current version of a document.
  /// The document is identified using a document type and ID.
  /// The Id of the entity itself may serve as a group Id for the whole row of versions of a specific DocType+DocId pair.
  /// </summary>
  public class AuditCurrentVersionEntity : FullAuditedAggregateRoot<Guid>
  {
    /// <summary>
    /// Gets or sets the current version of the document referenced by the DocType+DocId pair.
    /// </summary>
    public virtual string CurrentVersion { get; set; }

    /// <summary>
    /// Gets or sets the type of the referenced document.
    /// </summary>
    public virtual string RefDocumentType { get; set; }

    /// <summary>
    /// Gets or sets the Id of the referenced document.
    /// May be any string, i.e. stringified Guid or any other field
    /// that will allow you to distinguish the document.
    /// </summary>
    public virtual string RefDocId { get; set; }

    /// <summary>
    /// Gets or sets the name of the person who changed the current version the last time.
    /// </summary>
    public virtual string LastModifierName { get; set; }

    protected AuditCurrentVersionEntity() { }

    public AuditCurrentVersionEntity(Guid id)
    {
      Id = id;
    }
  }
}
