using Eleon.Storage.Lib.Constants;
using SharedModule.modules.Blob.Module.Constants;
using System.Collections.Generic;
using VPortal.Storage.Module.Entities;

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

    public static readonly Dictionary<string, List<StorageProviderSettingTypeEntity>> PossibleSettings =
    new()
    {
      [StorageProviderDomainConstants.StorageTypeDatabase] = new()
        {
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeDatabase,
                Type = StorageProviderSettingsTypes.String,
                Key = "ConnectionString",
                DefaultValue = "",
                Description = "Database connection string",
                Hidden = false,
                Required = true
            },
        },

      [StorageProviderDomainConstants.StorageTypeAWS] = new()
        {
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAWS,
                Key = "AccessKeyId",
                Type = StorageProviderSettingsTypes.String,
                Description = "AWS Access Key ID",
                Hidden = false,
                Required = true
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAWS,
                Key = "SecretAccessKey",
                Type = StorageProviderSettingsTypes.String,
                Description = "AWS Secret Access Key",
                Hidden = true,
                Required = true
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAWS,
                Key = "UseCredentials",
                Type = StorageProviderSettingsTypes.Boolean,
                Description = "Use stored AWS credentials",
                DefaultValue = "false",
                Hidden = false,
                Required = false
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAWS,
                Key = "UseTemporaryCredentials",
                Type = StorageProviderSettingsTypes.Boolean,
                Description = "Use temporary AWS credentials",
                DefaultValue = "false",
                Hidden = false,
                Required = false
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAWS,
                Key = "UseTemporaryFederatedCredentials",
                Type = StorageProviderSettingsTypes.Boolean,
                Description = "Use federated temporary AWS credentials",
                DefaultValue = "false",
                Hidden = false,
                Required = false
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAWS,
                Key = "ProfileName",
                Type = StorageProviderSettingsTypes.String,
                Hidden = false,
                Required = false
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAWS,
                Key = "ProfilesLocation",
                Type = StorageProviderSettingsTypes.String,
                Hidden = false,
                Required = false
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAWS,
                Key = "Region",
                Type = StorageProviderSettingsTypes.String,
                Hidden = false,
                Required = true
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAWS,
                Key = "ContainerName",
                Type = StorageProviderSettingsTypes.String,
                Hidden = false,
                Required = true
            }
        },

      [StorageProviderDomainConstants.StorageTypeAzure] = new()
        {
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAzure,
                Key = "ConnectionString",
                Type = StorageProviderSettingsTypes.String,
                Required = true
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeAzure,
                Key = "ContainerName",
                Type = StorageProviderSettingsTypes.String,
                Required = true
            }
        },

      [StorageProviderDomainConstants.StorageTypeFileSystem] = new()
        {
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeFileSystem,
                Key = "BasePath",
                Type = StorageProviderSettingsTypes.String,
                Required = false,
                DefaultValue = "./"
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeFileSystem,
                Key = "AppendContainerNameToBasePath",
                Type = StorageProviderSettingsTypes.Boolean,
                Required = false,
                DefaultValue = "false"
            },
             new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeFileSystem,
                Key = "IsMultitenancyEnabled",
                Type = StorageProviderSettingsTypes.Boolean,
                Required = false,
                DefaultValue = "false"
            }
        },

      [StorageProviderDomainConstants.StorageTypeProxy] = new()
        {
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeProxy,
                Key = "ProxyId",
                Type = StorageProviderSettingsTypes.String,
                Required = true
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeProxy,
                Key = "BasePath",
                Type = StorageProviderSettingsTypes.String,
                Required = true
            },
        },

      [StorageProviderDomainConstants.StorageTypeSFTP] = new()
        {
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeSFTP,
                Key = "Host",
                Type = StorageProviderSettingsTypes.String,
                Required = true
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeSFTP,
                Key = "Port",
                Type = StorageProviderSettingsTypes.Integer,
                DefaultValue = "22",
                Required = true
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeSFTP,
                Key = "UserName",
                Type = StorageProviderSettingsTypes.String,
                Required = true
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeSFTP,
                Key = "Password",
                Type = StorageProviderSettingsTypes.String,
                Hidden = true,
                Required = false
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeSFTP,
                Key = "BasePath",
                Type = StorageProviderSettingsTypes.String,
                Required = false,
                DefaultValue = "./"
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeSFTP,
                Key = "UsePrivateKey",
                Type = StorageProviderSettingsTypes.Boolean,
                Required = false,
                DefaultValue = "false"
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeSFTP,
                Key = "PrivateKey",
                Type = StorageProviderSettingsTypes.MultilineString,
                Required = false,
                Hidden = true
            },
            new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeFileSystem,
                Key = "IsMultitenancyEnabled",
                Type = StorageProviderSettingsTypes.Boolean,
                DefaultValue = "false",
                Required = false
            },
             new(Guid.Empty)
            {
                StorageProviderTypeName = StorageProviderDomainConstants.StorageTypeFileSystem,
                Key = "AppendContainerNameToBasePath",
                Type = StorageProviderSettingsTypes.Boolean,
                Required = false,
                DefaultValue = "false"
            },
        }
    };

  }
}
