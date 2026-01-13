namespace TenantSettings.Module.Models
{
  public class UserIsolationSettings
  {
    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
    public bool UserIsolationEnabled { get; set; }
    public string UserCertificateHash { get; set; }
    public bool TenantIsolationEnabled { get; set; }
  }
}
