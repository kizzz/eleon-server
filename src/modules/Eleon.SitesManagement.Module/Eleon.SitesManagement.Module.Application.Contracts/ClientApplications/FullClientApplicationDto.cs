using System.Collections.Generic;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.ClientApplications
{
  public class FullClientApplicationDto : ClientApplicationDto
  {
    // domains
    // parent dto
    // children dto
    public List<ApplicationModuleDto> Modules { get; set; }
  }
}


