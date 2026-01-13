using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using VPortal.FileManager.Module.Constants;

namespace VPortal.FileManager.Module.Files
{
  public class EntryFilterDto
  {
    public Guid ArchiveId { get; set; }
    public FileManagerType FileManagerType { get; set; }
    public EntryKind? Kind { get; set; } // Optional: filter by EntryKind
    public bool FilterByFavourite { get; set; }
    public bool FilterByStatus { get; set; }
    public bool FilterByShareStatus { get; set; } // Only applies to files
    public List<FileStatus> FileStatuses { get; set; }
    public List<FileShareStatus> FileShareStatuses { get; set; } // Only applies to files
  }
}

