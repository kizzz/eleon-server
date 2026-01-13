using System;
using Volo.Abp.BlobStoring;

namespace Storage.Module.BlobProviders.GoogleDrive
{
  public static class GoogleDriveBlobProviderConfigurationExtensions
  {
    public static BlobContainerConfiguration UseGoogleDrive(
        this BlobContainerConfiguration containerConfiguration,
        Action<GoogleDriveBlobContainerConfiguration> configureAction)
    {
      containerConfiguration.ProviderType = typeof(GoogleDriveBlobProvider);

      configureAction?.Invoke(containerConfiguration.GetGoogleDriveBlobProviderConfiguration());

      return containerConfiguration;
    }

    public static GoogleDriveBlobContainerConfiguration GetGoogleDriveBlobProviderConfiguration(
        this BlobContainerConfiguration containerConfiguration)
    {
      return new GoogleDriveBlobContainerConfiguration(containerConfiguration);
    }
  }
}
