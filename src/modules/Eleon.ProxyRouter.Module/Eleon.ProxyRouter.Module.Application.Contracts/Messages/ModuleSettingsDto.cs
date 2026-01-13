using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyRouter.Minimal.HttpApi.Models.Messages
{
  public class ModuleSettingsDto
  {
    public Guid? TenantId { get; set; }
    public Guid? SiteId { get; set; }
    public required List<ClientApplicationDto> ClientApplications { get; set; }
    public required List<EleoncoreModuleDto> Modules { get; set; }
  }
}
