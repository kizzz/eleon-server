using System;
using Volo.Abp.BlobStoring;

namespace Storage.Module.BlobProviders.GoogleDrive
{
  public class GoogleDriveBlobContainerConfiguration
  {

    public string SettingsGroup
    {
      get => _containerConfiguration.GetConfiguration<string>("SettingsGroup");
      set => _containerConfiguration.SetConfiguration("SettingsGroup", value);
    }

    private readonly BlobContainerConfiguration _containerConfiguration;

    public GoogleDriveBlobContainerConfiguration(
        BlobContainerConfiguration containerConfiguration)
    {
      _containerConfiguration = containerConfiguration;
    }

    private Guid ParseOrDefault(string str)
    {
      if (Guid.TryParse(str, out var guid))
      {
        return guid;
      }

      return Guid.Empty;
    }
  }
}
