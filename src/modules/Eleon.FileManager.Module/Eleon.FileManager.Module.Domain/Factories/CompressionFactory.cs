using Common.Module.Constants;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.DependencyInjection;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Repositories.Compression;

namespace VPortal.FileManager.Module.Factories
{
  public class CompressionFactory : ICompressionFactory, ITransientDependency
  {
    private readonly IServiceProvider serviceProvider;

    public CompressionFactory(
        IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }
    public ICompressionRepository Get(FileCompressionType fileCompressionType = FileCompressionType.Zip)
        => fileCompressionType switch
        {
          FileCompressionType.Zip => serviceProvider.GetRequiredService<ZipCompressionRepository>(),
          _ => null,
        };
  }
}
