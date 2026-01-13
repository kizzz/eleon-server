using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages;

[Common.Module.Events.DistributedEvent]
public class ModuleSettingsGotMsg : VportalEvent
{
  public Guid? SiteId { get; set; }
  public List<ClientApplicationEto> ClientApplications { get; set; }
  public List<EleoncoreModuleEto> Modules { get; set; }
}


[Common.Module.Events.DistributedEvent]
public class ModuleSettingsRefreshedMsg : ModuleSettingsGotMsg
{
}
