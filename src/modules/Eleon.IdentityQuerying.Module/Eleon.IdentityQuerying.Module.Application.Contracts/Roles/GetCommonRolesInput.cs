using Volo.Abp.Application.Dtos;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles
{
  public class GetCommonRolesInput : PagedAndSortedResultRequestDto
  {
    public string Filter { get; set; }
  }
}
