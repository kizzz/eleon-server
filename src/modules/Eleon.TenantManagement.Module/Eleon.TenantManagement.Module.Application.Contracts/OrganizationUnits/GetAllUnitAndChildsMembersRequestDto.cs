

using Volo.Abp.Application.Dtos;

namespace VPortal.TenantManagement.Module.OrganizationUnits;
public class GetAllUnitAndChildsMembersRequestDto : PagedAndSortedResultRequestDto
{
  public Guid OrgUnitId { get; set; }
  public string SearchQuery { get; set; }
}
