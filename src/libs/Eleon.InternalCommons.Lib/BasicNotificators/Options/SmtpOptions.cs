using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
public class SmtpOptions
{
  public string DefaultFromAddress { get; set; }
  public string DefaultFromDisplayName { get; set; }
  public string Host { get; set; }
  public int Port { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
  public string Domain { get; set; }
  public bool EnableSsl { get; set; }
  public bool UseDefaultCredentials { get; set; }

  public SmtpOptions() { }

  public SmtpOptions(
      string defaultFromAddress,
      string defaultFromDisplayName,
      string host,
      int port,
      string username,
      string password,
      string domain,
      bool enableSsl,
      bool useDefaultCredentials)
  {
    DefaultFromAddress = defaultFromAddress;
    DefaultFromDisplayName = defaultFromDisplayName;
    Host = host;
    Port = port;
    Username = username;
    Password = password;
    Domain = domain;
    EnableSsl = enableSsl;
    UseDefaultCredentials = useDefaultCredentials;
  }
}

