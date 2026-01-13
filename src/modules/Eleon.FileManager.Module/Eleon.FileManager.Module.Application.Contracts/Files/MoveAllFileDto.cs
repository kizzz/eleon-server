using System.Collections.Generic;

namespace VPortal.FileManager.Module.Files
{
  public class MoveAllFileDto
  {
    public List<string> FileIds { get; set; }
    public List<string> Folders { get; set; }
    public string DestinationFolderId { get; set; }
  }
}
