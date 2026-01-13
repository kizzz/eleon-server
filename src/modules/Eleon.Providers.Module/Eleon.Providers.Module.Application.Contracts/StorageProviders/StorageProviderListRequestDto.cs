using Volo.Abp.Application.Dtos;

namespace Storage.Module.StorageProviders
{
  public class StorageProviderListRequestDto : PagedAndSortedResultRequestDto
  {
    public string SearchQuery { get; set; }
  }
}
