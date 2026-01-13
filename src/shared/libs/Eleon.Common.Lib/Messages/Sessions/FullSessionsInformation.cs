using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.EleonsoftIdentity.Sessions;
public class FullSessionInformation
{
  public string SessionId { get; set; }

  public class UserInfo
  {
    public bool IsAuthenticated { get; set; }

    public Guid? Id { get; set; }

    public string? UserName { get; set; }

    public string? Name { get; set; }

    public string? SurName { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberVerified { get; set; }

    public string? Email { get; set; }

    public bool EmailVerified { get; set; }

    public Guid? TenantId { get; set; }

    public string[] Roles { get; set; }

    public Claim[] Claims { get; set; }

    public override string ToString()
    {
      return
          $"IsAuthenticated: {IsAuthenticated}\n" +
          $"Id: {Id}\n" +
          $"UserName: {UserName}\n" +
          $"Email: {Email}\n" +
          $"Roles: [{(Roles != null ? string.Join(", ", Roles) : "")}]\n";
    }
  }

  public UserInfo User { get; set; }

  public class TenantInfo
  {
    public Guid? TenantId { get; set; }
    public string? Name { get; set; }

    public override string ToString()
    {
      return $"TenantId: {TenantId}\nName: {Name}\n";
    }
  }

  public TenantInfo Tenant { get; set; }

  public class RequestInfo
  {
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string ParsedDevice { get; set; }
    public string Host { get; set; }
    public string XForwardedFor { get; set; }

    public override string ToString()
    {
      return
          $"IpAddress: {IpAddress}\n" +
          $"UserAgent: {UserAgent}\n" +
          $"ParsedDevice: {ParsedDevice}\n" +
          $"Domain: {Host}\n" +
          $"X-Forwarded-For: {XForwardedFor}\n";
    }
  }

  public RequestInfo Request { get; set; }

  public override string ToString()
  {
    return
        $"SessionId: {SessionId ?? "Unknown"}\n" +
        $"User:\n\t{User?.ToString() ?? "Unknown"}\n" +
        $"Tenant:\n\t{Tenant?.ToString() ?? "Unknown"}\n" +
        $"Request:\n\t{Request?.ToString() ?? "Unknown"}\n";
  }
}
