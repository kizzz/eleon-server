using Common.Module.Constants;

namespace VPortal.TenantManagement.Module.UserSettings
{
  public class UserSettingDto
  {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public TwoFaNotificationType? TwoFaNotificationType { get; set; }
  }
}
