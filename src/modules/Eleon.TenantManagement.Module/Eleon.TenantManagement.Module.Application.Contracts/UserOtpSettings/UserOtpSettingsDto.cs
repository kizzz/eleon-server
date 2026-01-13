using Common.Module.Constants;
using System;

namespace VPortal.TenantManagement.Module.UserOtpSettings
{
  public class UserOtpSettingsDto
  {
    public UserOtpType UserOtpType { get; set; }
    public string OtpEmail { get; set; }
    public string OtpPhoneNumber { get; set; }
    public Guid UserId { get; set; }
  }
}
