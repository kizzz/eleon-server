using Volo.Abp.Application.Dtos;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Users
{
  public class GetCommonUsersInput : PagedAndSortedResultRequestDto
  {
    public string Filter { get; set; }
    public string Permissions { get; set; }
    public bool IgnoreCurrentUser { get; set; }
    public List<Guid> IgnoredUsers { get; set; }
  }
}
