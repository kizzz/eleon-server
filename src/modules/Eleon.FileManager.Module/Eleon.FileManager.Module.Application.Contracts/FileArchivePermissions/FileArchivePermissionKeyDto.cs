using ModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System;

namespace VPortal.FileManager.Module.FileArchivePermissions
{
  public class FileArchivePermissionKeyDto
  {
    public Guid ArchiveId { get; set; }
    public string FolderId { get; set; }
    public PermissionActorType ActorType { get; set; }
    public string ActorId { get; set; }
    public string UniqueKey { get; set; }
  }
}
