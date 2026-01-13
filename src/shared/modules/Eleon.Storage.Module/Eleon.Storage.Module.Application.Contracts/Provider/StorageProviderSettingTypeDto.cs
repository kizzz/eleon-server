using Eleon.Storage.Lib.Constants;
using Eleon.Storage.Lib.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModule.modules.Blob.Module.Models;

public class StorageProviderSettingTypeDto
{
  public string StorageProviderTypeName { get; set; }
  public StorageProviderSettingsTypes Type { get; set; }
  public string Key { get; set; }
  public string DefaultValue { get; set; }
  public string Description { get; set; }
  public bool Hidden { get; set; }
  public bool Required { get; set; }
}
