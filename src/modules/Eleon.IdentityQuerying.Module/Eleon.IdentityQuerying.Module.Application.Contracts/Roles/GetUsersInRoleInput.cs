namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles
{
  public class GetUsersInRoleInput
  {
    public string RoleName { get; set; }
    public string UserNameFilter { get; set; }
    public int SkipCount { get; set; }
    public int MaxResultCount { get; set; }
    public bool ExclusionMode { get; set; }
  }
}
