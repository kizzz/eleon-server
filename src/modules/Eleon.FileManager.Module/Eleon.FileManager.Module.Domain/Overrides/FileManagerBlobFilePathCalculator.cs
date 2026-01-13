using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using VPortal.FileManager.Module.Settings;

namespace VPortal.FileManager.Module.Overrides
{
  public class FileManagerBlobFilePathCalculator : DefaultBlobFilePathCalculator, ITransientDependency
  {
    private readonly FileStorageSettingProvider fileStorageSettingProvider;

    protected new ICurrentTenant CurrentTenant { get; }

    public FileManagerBlobFilePathCalculator(ICurrentTenant currentTenant,
        FileStorageSettingProvider fileStorageSettingProvider)
        : base(currentTenant)
    {
      CurrentTenant = currentTenant;
      this.fileStorageSettingProvider = fileStorageSettingProvider;
    }

    public override string Calculate(BlobProviderArgs args)
    {
      if (string.IsNullOrEmpty(fileStorageSettingProvider.Path))
      {
        return base.Calculate(args);
      }

      return fileStorageSettingProvider.Path;
    }
  }
}
