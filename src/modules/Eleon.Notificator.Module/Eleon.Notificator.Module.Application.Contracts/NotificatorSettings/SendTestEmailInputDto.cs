using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.Emails;
public class SendTestEmailInputDto
{
  [Required]
  public string SenderEmailAddress { get; set; }

  [Required]
  public string TargetEmailAddress { get; set; }

  [Required]
  public string Subject { get; set; }

  public string Body { get; set; }
}
