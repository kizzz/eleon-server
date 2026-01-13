using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;

namespace VPortal.FileManager.Module.Files
{
  public class DownloadAllDto
  {
    public List<string> FileIds { get; set; }
    public List<string> Folders { get; set; }
    public Guid ArchiveId { get; set; }
    public FileManagerType FileManagerType { get; set; }
    public string ParentId { get; set; }
  }
}
