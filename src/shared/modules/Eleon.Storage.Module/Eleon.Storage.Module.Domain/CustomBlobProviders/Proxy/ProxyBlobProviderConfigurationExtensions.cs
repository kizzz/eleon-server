using System;
using Volo.Abp.BlobStoring;

namespace Storage.Module.BlobProviders.Proxy
{
  //public static class ProxyBlobProviderConfigurationExtensions
  //{
  //    public static BlobContainerConfiguration UseProxyBlobProvider(
  //        this BlobContainerConfiguration containerConfiguration,
  //        Action<ProxyBlobContainerConfiguration> configureAction)
  //    {
  //        containerConfiguration.ProviderType = typeof(ProxyBlobProvider);

  //        configureAction?.Invoke(containerConfiguration.GetProxyBlobProviderConfiguration());

  //        return containerConfiguration;
  //    }

  //    public static ProxyBlobContainerConfiguration GetProxyBlobProviderConfiguration(
  //        this BlobContainerConfiguration containerConfiguration)
  //    {
  //        return new ProxyBlobContainerConfiguration(containerConfiguration);
  //    }
  //}
}
