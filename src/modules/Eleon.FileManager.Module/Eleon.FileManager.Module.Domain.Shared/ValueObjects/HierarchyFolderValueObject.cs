using Common.Module.Constants;
using System;
using System.Collections.Generic;

namespace VPortal.FileManager.Module.ValueObjects
{
  public class HierarchyFolderValueObject
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public Guid PhysicalFolderId { get; set; }
    public FileStatus Status { get; set; }
    public string Path { get; set; }
    public bool IsShared { get; set; }
    public bool IsFavourite { get; set; }
    public List<HierarchyFolderValueObject> Children { get; set; } = new List<HierarchyFolderValueObject>();
    public List<FileValueObject> Files { get; set; }
  }
}
