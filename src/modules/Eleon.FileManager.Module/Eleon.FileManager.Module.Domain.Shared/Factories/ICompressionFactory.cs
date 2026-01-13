using Common.Module.Constants;
using Volo.Abp.DependencyInjection;
using VPortal.FileManager.Module.Repositories;

namespace VPortal.FileManager.Module.Factories
{
  public interface ICompressionFactory : ITransientDependency
  {
    public ICompressionRepository Get(FileCompressionType fileCompressionType = FileCompressionType.Zip);
  }
}
