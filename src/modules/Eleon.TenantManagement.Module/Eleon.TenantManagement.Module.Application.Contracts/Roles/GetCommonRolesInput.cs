using Volo.Abp.Application.Dtos;

namespace VPortal.TenantManagement.Module.Roles
{
  public class GetCommonRolesInput : PagedAndSortedResultRequestDto
  {
    public string Filter { get; set; }
  }
}
