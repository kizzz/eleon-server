using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.VirtualFolders
{
  public class RenameVirtualFolderDto : EntityDto<string>
  {
    public string Name { get; set; }
  }
}
