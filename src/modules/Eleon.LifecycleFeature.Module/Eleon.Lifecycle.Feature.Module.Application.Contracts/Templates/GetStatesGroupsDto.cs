using Common.Module.Constants;
using Volo.Abp.Application.Dtos;

namespace VPortal.Lifecycle.Feature.Module.Templates
{
  public class GetStatesGroupsDto : PagedAndSortedResultRequestDto
  {
    public string DocumentObjectType { get; set; }
  }
}
