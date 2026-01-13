using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.OrganizationUnits;
using System;
using System.Collections.Generic;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Users
{
  public class CommonUserDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public IList<string> Roles { get; set; }
    public List<CommonOrganizationUnitDto> OrganizationUnits { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string ProfilePicture { get; set; }
    public string ProfilePictureThumbnail { get; set; }
    public bool IsActive { get; set; }
  }
}
