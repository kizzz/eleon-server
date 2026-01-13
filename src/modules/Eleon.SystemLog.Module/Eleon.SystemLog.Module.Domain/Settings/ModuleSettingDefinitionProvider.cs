using Volo.Abp.Settings;

namespace VPortal.DocMessageLog.Module.Settings;

public class ModuleSettingDefinitionProvider : SettingDefinitionProvider
{
  public override void Define(ISettingDefinitionContext context)
  {
    context.Add(new SettingDefinition(
        name: ModuleSettings.LastReadedLogState,
        defaultValue: null,
        isVisibleToClients: true
        ));
  }
}
