using Common.Module.Constants;
using System.Collections.Generic;

namespace VPortal.FileManager.Module.FileArchivePermissions
{
  public class FileArchivePermissionDto : FileArchivePermissionKeyDto
  {
    public IEnumerable<FileManagerPermissionType> AllowedPermissions { get; set; }
    public string ActorDisplayName { get; set; }
  }
}
