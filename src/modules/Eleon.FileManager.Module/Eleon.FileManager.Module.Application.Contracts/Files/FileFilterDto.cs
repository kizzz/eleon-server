using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;

namespace VPortal.FileManager.Module.Files
{
  public class FileFilterDto
  {
    public Guid ArchiveId { get; set; }
    public FileManagerType FileManagerType { get; set; }
    public bool FilterByFavourite { get; set; }
    public bool FilterByStatus { get; set; }
    public bool FilterByShareStatus { get; set; }
    public List<FileStatus> FileStatuses { get; set; }
    public List<FileShareStatus> FileShareStatuses { get; set; }
  }
}
