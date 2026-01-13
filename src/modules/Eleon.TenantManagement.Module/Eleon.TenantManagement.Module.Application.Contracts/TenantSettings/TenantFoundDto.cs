using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.TenantManagement.Module.TenantSettings
{
  public class TenantFoundDto
  {
    public bool IsFound { get; set; }
    public Guid? TenantId { get; set; }
  }
}
