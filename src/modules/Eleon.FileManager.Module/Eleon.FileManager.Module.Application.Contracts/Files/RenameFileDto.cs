using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.Files
{
  public class RenameFileDto : EntityDto<string>
  {
    public string Name { get; set; }
  }
}
