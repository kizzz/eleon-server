using System;

namespace VPortal.Identity.Module.Entities;

public class UserSessionEto
{
  public string Id { get; set; }
  public string UserId { get; set; }
  public string Device { get; set; }
  public object? DeviceInfo { get; set; }
  public string Browser { get; set; }
  public string IpAddress { get; set; }
  public DateTime SignInDate { get; set; }
  public DateTime? LastAccessTime { get; set; }
  public DateTime? Expiration { get; set; }
}
