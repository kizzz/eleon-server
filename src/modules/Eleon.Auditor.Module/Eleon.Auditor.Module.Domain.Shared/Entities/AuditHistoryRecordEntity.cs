using Common.Module.Constants;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.Auditor.Module.Entities;

/// <summary>
/// The entity represents a single record in the audit history of a document referenced by its properties.
/// The data of the document is separated to <see cref="AuditDataEntity"/> so that it wont duplicate when copying the history records.
/// </summary>
public class AuditHistoryRecordEntity : CreationAuditedAggregateRoot<Guid>
{
  /// <summary>
  /// Gets or sets the internal Id of an entity that contains the current version of the referenced document.
  /// While the DocumentId and DocumentObjectType fields identify the document whose data is referenced by this entity,
  /// the AuditVersionId allows to group by the Reference document (see <see cref="AuditCurrentVersionEntity"/>).
  /// The string representation of the version that this record holds is stored in <see cref="Version"/>.
  /// </summary>
  public virtual Guid AuditVersionId { get; set; }

  /// <summary>
  /// Gets or sets the version which is represented by referenced audit data.
  /// </summary>
  public virtual string Version { get; set; }

  /// <summary>
  /// Gets or sets the type of the document whose data is referenced by this entity.
  /// </summary>
  public virtual string DocumentObjectType { get; set; }

  /// <summary>
  /// Gets or sets the ID of the document whose data is referenced by this entity.
  /// </summary>
  public virtual string DocumentId { get; set; }

  /// <summary>
  /// Gets or sets the reference to an <see cref="AuditDataEntity"/> that contains the document data.
  /// </summary>
  public virtual Guid AuditDataId { get; set; }

  /// <summary>
  /// Gets or sets the Id of the transaction during which this record was created.
  /// </summary>
  public virtual string TransactionId { get; set; }

  /// <summary>
  /// Gets or sets the name of the person who created this audit record.
  /// </summary>
  public string CreatorName { get; set; }

  protected AuditHistoryRecordEntity() { }

  public AuditHistoryRecordEntity(Guid id)
  {
    Id = id;
  }
}
