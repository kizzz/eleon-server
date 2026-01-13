using Microsoft.AspNetCore.Authorization;

namespace Volo.Abp.Authorization;

public class EleonsoftSdkPermissionRequirement : IAuthorizationRequirement
{
  public string PermissionName { get; }

  public EleonsoftSdkPermissionRequirement(string permissionName)
  {
    PermissionName = permissionName;
  }

  public override string ToString()
  {
    return $"PermissionRequirement: {PermissionName}";
  }
}
