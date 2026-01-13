using Volo.Abp.Settings;

namespace VPortal.Settings;

public class ProxyClientSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(VPortalSettings.MySetting1));

        context.Add(new SettingDefinition(ProxyClientSettings.Debug, "false"));
        context.Add(new SettingDefinition(ProxyClientSettings.FactoryUseMock, "false"));
    }
}
