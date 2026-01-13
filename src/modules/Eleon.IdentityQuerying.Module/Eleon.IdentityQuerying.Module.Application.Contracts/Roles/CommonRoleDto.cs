using System;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles
{
  public class CommonRoleDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsReadOnly { get; set; }
    public string InheritedFrom { get; set; }
    public bool IsDefault { get; set; }
    public bool IsPublic { get; set; }
  }
}
