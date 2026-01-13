using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;

namespace VPortal.FileManager.Module.Files
{
  public class FileUploadDto
  {
    public List<FileSourceDto> Files { get; set; }
    public Guid ArchiveId { get; set; }
    public FileManagerType FileManagerType { get; set; }
    public string FolderId { get; set; }
  }
}
