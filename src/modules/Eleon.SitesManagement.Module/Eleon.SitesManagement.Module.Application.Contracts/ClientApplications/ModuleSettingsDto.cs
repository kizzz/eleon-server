using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.ClientApplications
{
  public class ModuleSettingsDto
  {
    public Guid? TenantId { get; set; }
    public Guid? SiteId { get; set; }
    public List<FullClientApplicationDto> ClientApplications { get; set; }
    public List<EleoncoreModuleDto> Modules { get; set; }
  }
}


