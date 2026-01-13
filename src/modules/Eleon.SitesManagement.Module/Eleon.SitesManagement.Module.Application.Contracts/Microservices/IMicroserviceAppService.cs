using Messaging.Module.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.SitesManagement.Module.Microservices
{
  public interface IMicroserviceAppService : IApplicationService
  {
    Task<bool> InitializeMicroservice(InitializeMicroserviceMsg initializeMicroserviceMsg);
    Task<List<EleoncoreModuleDto>> GetMicroserviceList();
    Task<EleoncoreModuleDto> Create(EleoncoreModuleDto applicationModuleDto);
  }
}


