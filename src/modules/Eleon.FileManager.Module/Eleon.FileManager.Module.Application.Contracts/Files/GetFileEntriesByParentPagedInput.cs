using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.Files
{
  public class GetFileEntriesByParentPagedInput : PagedAndSortedResultRequestDto
  {
    public string FolderId { get; set; }
  }
}


