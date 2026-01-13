using Common.Module.Constants;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using VPortal.Accounting.Module.Localization;

namespace VPortal.Accounting.Module.Permissions;

public class AccountingPermissionDefinitionProvider : PermissionDefinitionProvider
{
  public override void Define(IPermissionDefinitionContext context)
  {
    var myGroup = context.AddGroup(
        AccountingPermissions.GroupName,
        L($"Permission:{FeaturePack.Account}"));

    myGroup.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Accounting");

    var general = myGroup.AddPermission(
        AccountingPermissions.General,
        L($"Permission:{FeaturePack.Account}:General"));
    general.Properties.Add("Order", 0);

    var create = general.AddChild(
        AccountingPermissions.Create,
        L($"Permission:{FeaturePack.Account}:Create"));
    create.Properties.Add("Order", 0);

    var accountManager = general.AddChild(
        AccountingPermissions.AccountManager,
        L($"Permission:{FeaturePack.Account}:AccountManager"));
    accountManager.Properties.Add("Order", 1);

    //var myGroup2 = context.AddGroup(
    //    AccountingPermissions.GroupName2,
    //    L($"Permission:{FeaturePack.Reseller}"));

    //myGroup2.Properties.Add("CategoryName", "TenantManagement::PermissionGroupCategory:Accounting");

    //var general2 = myGroup2.AddPermission(
    //    AccountingPermissions.General2,
    //    L($"Permission:{FeaturePack.Reseller}:General"));
    //general2.Properties.Add("Order", 0);

    //var create2 = general2.AddChild(
    //    AccountingPermissions.Create2,
    //    L($"Permission:{FeaturePack.Reseller}:Create"));
    //create2.Properties.Add("Order", 0);

  }

  private static LocalizableString L(string name)
  {
    return LocalizableString.Create<AccountingResource>(name);
  }
}
