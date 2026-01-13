using Eleon.Storage.Lib.Constants;
using SharedModule.modules.Blob.Module.Constants;
using System.Collections.Generic;

namespace VPortal.Storage.Module
{
  internal static class StorageDomainConsts
  {
    public const string StorageProviderTypeSeparator = ", ";
    public const string StorageProviderSettingKey = "STORAGE_PROVIDER";
    public const string ExplicitKeySettingGroupPrefix = "[EXPLICIT]";
    public const string ExplicitProviderTypeSettingsGroup = "[EXPLICIT_TYPE]";
    public const string TestSettingGroupPrefix = "[TEST]";
    public const string TestBlobNamePrefix = "[TEST]";
    public const string TestStringPrefix = "[TEST]";
    public const string DefaultContainerName = "default";
    public const string CensoredValue = "********";
  }
}
