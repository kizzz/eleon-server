using System;

namespace VPortal.FileManager.Module.FileArchiveFavourites
{
  public class FileArchiveFavouriteDto
  {
    public Guid ArchiveId { get; set; }
    public string ParentId { get; set; }
    public string UserId { get; set; }
    public string FileId { get; set; }
    public string FolderId { get; set; }
  }
}
