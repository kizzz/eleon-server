using Common.Module.Constants;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.JobScheduler.Module.Localization;

namespace VPortal.JobScheduler.Module.Permissions;

public class ModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var group = context.AddGroup(JobSchedulerPermissions.GroupName, L($"Permission:JobSchedule"));
    group.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Administration");
    var general = group.AddPermission(
        JobSchedulerPermissions.General,
        L($"Permission:JobSchedule:General"));
    general.Properties.Add("Order", 0);
    var create = general.AddChild(
        JobSchedulerPermissions.Create,
        L($"Permission:JobSchedule:Create"));
    create.Properties.Add("Order", 0);
  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<JobSchedulerModuleResource>(name);
  }
}
