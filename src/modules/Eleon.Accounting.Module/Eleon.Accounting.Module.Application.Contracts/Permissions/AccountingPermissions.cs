using Common.Module.Constants;
using Volo.Abp.Reflection;

namespace VPortal.Accounting.Module.Permissions;

public class AccountingPermissions
{
  public const string GroupName = "AccountingModule";
  public const string General = $"Permission.{nameof(FeaturePack.Account)}.General";
  public const string Create = $"Permission.{nameof(FeaturePack.Account)}.Create";
  public const string AccountManager = $"Permission.{nameof(FeaturePack.Account)}.AccountManager";

  public const string GroupName2 = "Reseller";
  public const string General2 = $"Permission.{nameof(FeaturePack.Reseller)}.General";
  public const string Create2 = $"Permission.{nameof(FeaturePack.Reseller)}.Create";

  public static string[] GetAll()
  {
    return ReflectionHelper.GetPublicConstantsRecursively(typeof(AccountingPermissions));
  }
}
