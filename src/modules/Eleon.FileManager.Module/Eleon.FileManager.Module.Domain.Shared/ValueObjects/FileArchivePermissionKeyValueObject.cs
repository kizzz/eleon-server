using ModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System;

namespace VPortal.FileManager.Module.ValueObjects
{
  public class FileArchivePermissionKeyValueObject
  {
    public Guid ArchiveId { get; set; }
    public string FolderId { get; set; }
    public PermissionActorType ActorType { get; set; }
    public string ActorId { get; set; }
    public string UniqueKey { get { return ActorType.ToString() + ActorId; } }
  }
}
