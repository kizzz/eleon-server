using Common.Module.Constants;

namespace Messaging.Module.Messages;

[Common.Module.Events.DistributedEvent]
public class SyncDocumentWithLifecycleMsg : VportalEvent
{
  public SyncDocumentWithLifecycleMsg()
  {
  }


  public string DocumentObjectType { get; set; }
  public string DocumentId { get; set; }

  public Guid GroupAuditId { get; set; }
  public LifecycleFinishedStatus Status { get; set; }
  public DateTime StatusDate { get; set; }
  public string Reason { get; set; }
  public string InitiatorDocEntry { get; set; }

  //#region IMessage implementation
  //public ClaimsPrincipalData Principal { get; set; }

  //public string CorrelationString { get; set; }
  //public DocumentObjectType? ObjectType { get; set; }

  //#endregion
}