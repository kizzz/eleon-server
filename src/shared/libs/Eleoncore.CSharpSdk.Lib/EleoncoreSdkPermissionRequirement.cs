using Microsoft.AspNetCore.Authorization;

namespace Volo.Abp.Authorization;

public class EleoncoreSdkPermissionRequirement : IAuthorizationRequirement
{
  public string PermissionName { get; }

  public EleoncoreSdkPermissionRequirement(string permissionName)
  {
    PermissionName = permissionName;
  }

  public override string ToString()
  {
    return $"PermissionRequirement: {PermissionName}";
  }
}
