using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Models;
using System.Collections.Generic;

namespace VPortal.Storage.Module.StorageProviders
{
  public class PossibleStorageProviderSettingsDto
  {
    public string Type { get; set; }
    public List<StorageProviderSettingTypeDto> PossibleSettings { get; set; }
  }
}
