using Volo.Abp.Application.Dtos;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.OrganizationUnits;
public class GetAllUnitAndChildsMembersRequestDto : PagedAndSortedResultRequestDto
{
  public Guid OrgUnitId { get; set; }
  public string SearchQuery { get; set; }
}
