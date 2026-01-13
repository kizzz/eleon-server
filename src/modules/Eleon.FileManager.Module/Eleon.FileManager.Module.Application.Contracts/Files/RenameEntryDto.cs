using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.Files
{
  public class RenameEntryDto : EntityDto<string>
  {
    public string Name { get; set; }
  }
}



