using Volo.Abp.DependencyInjection;

namespace VPortal.FileManager.Module.Settings
{
  public class FileStorageSettingProvider : IScopedDependency
  {
    public string Path { get; set; }
    public FileStorageSettingProvider()
    {

    }
  }
}
