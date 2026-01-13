using Core.Infrastructure.Module;
using Volo.Abp.Settings;

namespace VPortal.TenantManagement.Module.Settings;

public class TenantManagementSettingDefinitionProvider : SettingDefinitionProvider
{
  public override void Define(ISettingDefinitionContext context)
  {
    context.Add(new SettingDefinition(TenantManagementSettings.LastOrgUnit));
    context.Add(new SettingDefinition(TenantManagementSettings.LastDatasource));
    context.Add(new SettingDefinition(TenantManagementSettings.LastCompany));
    context.Add(new SettingDefinition(TenantManagementSettings.LastBuCompany));
    context.Add(new SettingDefinition(TenantManagementSettings.Appearance));
    context.Add(new SettingDefinition("SelectedUserCompany"));

    context.Add(new SettingDefinition(TenantManagementSettings.TenantSystemHealthSettings));
  }
}
