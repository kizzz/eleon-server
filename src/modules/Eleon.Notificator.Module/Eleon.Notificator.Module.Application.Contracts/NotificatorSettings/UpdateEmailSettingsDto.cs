using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.Emails;
public class UpdateEmailSettingsDto
{
  [MaxLength(256)]
  public string SmtpHost { get; set; }

  [Range(1, 65535)]
  public int SmtpPort { get; set; }

  [MaxLength(1024)]
  public string SmtpUserName { get; set; }

  [MaxLength(1024)]
  [DataType(DataType.Password)]
  [DisableAuditing]
  public string SmtpPassword { get; set; }

  [MaxLength(1024)]
  public string SmtpDomain { get; set; }

  public bool SmtpEnableSsl { get; set; }

  public bool SmtpUseDefaultCredentials { get; set; }

  [MaxLength(1024)]
  [Required]
  public string DefaultFromAddress { get; set; }

  [MaxLength(1024)]
  [Required]
  public string DefaultFromDisplayName { get; set; }
}

