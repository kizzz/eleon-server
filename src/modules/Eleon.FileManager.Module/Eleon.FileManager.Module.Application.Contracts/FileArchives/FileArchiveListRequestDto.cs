using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.FileArchives
{
  public class FileArchiveListRequestDto : PagedAndSortedResultRequestDto
  {
    public string SearchQuery { get; set; }
  }
}
