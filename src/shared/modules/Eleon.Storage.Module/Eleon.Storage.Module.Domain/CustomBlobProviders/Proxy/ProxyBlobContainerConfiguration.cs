using System;
using Volo.Abp.BlobStoring;

namespace Storage.Module.BlobProviders.Proxy
{
  public class ProxyBlobContainerConfiguration
  {
    public Guid ProxyId
    {
      get => ParseOrDefault(_containerConfiguration.GetConfiguration<string>("ProxyId"));
      set => _containerConfiguration.SetConfiguration("ProxyId", value.ToString());
    }

    public string SettingsGroup
    {
      get => _containerConfiguration.GetConfiguration<string>("SettingsGroup");
      set => _containerConfiguration.SetConfiguration("SettingsGroup", value);
    }

    private readonly BlobContainerConfiguration _containerConfiguration;

    public ProxyBlobContainerConfiguration(
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
