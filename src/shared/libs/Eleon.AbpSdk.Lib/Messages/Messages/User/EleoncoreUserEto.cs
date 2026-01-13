using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.Messages.User;
public class EleoncoreUserEto
{
  public Guid Id { get; set; }
  public Guid? TenantId { get; set; }
  public bool IsActive { get; set; } = true;
  public string UserName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Surname { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; }
}
