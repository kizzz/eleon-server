using Common.Module.Constants;
using System.Collections.Generic;

namespace VPortal.FileManager.Module.ValueObjects
{
  public class FileArchivePermissionValueObject : FileArchivePermissionKeyValueObject
  {
    public IEnumerable<FileManagerPermissionType> AllowedPermissions { get; set; }
    public string ActorDisplayName { get; set; }
  }
}
