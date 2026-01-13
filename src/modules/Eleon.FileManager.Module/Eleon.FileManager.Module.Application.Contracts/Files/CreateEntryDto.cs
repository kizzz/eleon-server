using VPortal.FileManager.Module.Constants;

namespace VPortal.FileManager.Module.Files
{
  public class CreateEntryDto
  {
    public EntryKind Kind { get; set; }
    public string Name { get; set; }
    public string? ParentId { get; set; }
    public string? Extension { get; set; }
    public string? PhysicalFolderId { get; set; }
    public bool IsShared { get; set; }
  }
}



