using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.Emails;
public class SmtpSettingsDto
{
  public string SmtpHost { get; set; }

  public int SmtpPort { get; set; }

  public string SmtpUserName { get; set; }

  public string SmtpPassword { get; set; }

  public string SmtpDomain { get; set; }

  public bool SmtpEnableSsl { get; set; }

  public bool SmtpUseDefaultCredentials { get; set; }

  public string DefaultFromAddress { get; set; }

  public string DefaultFromDisplayName { get; set; }
}
