using System.Collections.Generic;

namespace VPortal.FileManager.Module.Files
{
  public class MoveAllEntriesDto
  {
    public List<string> EntryIds { get; set; }
    public string DestinationParentId { get; set; }
  }
}



