using Common.Module.Constants;

namespace TenantSettings.Module.Models
{
  public class UserOtpSettings
  {
    public Guid? TenantId { get; set; }
    public UserOtpType UserOtpType { get; set; }
    public string OtpEmail { get; set; }
    public string OtpPhoneNumber { get; set; }
    public Guid UserId { get; set; }
  }
}
