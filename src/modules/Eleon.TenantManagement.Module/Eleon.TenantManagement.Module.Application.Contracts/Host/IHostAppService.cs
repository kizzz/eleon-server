using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Host
{

  public interface IHostAppService : IApplicationService
  {
    Task MigrateAsync(Guid id);
  }

}
