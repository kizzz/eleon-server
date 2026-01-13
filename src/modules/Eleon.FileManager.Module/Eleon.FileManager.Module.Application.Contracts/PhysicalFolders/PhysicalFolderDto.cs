using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.PhysicalFolders
{
  public class PhysicalFolderDto : EntityDto<string>
  {
    public string Name { get; set; }
    public long SystemFolderName { get; set; }
    public string ParentId { get; set; }
    public PhysicalFolderDto Parent { get; set; }
    public string Size { get; set; }
    public List<PhysicalFolderDto> Children { get; set; }
  }
}
