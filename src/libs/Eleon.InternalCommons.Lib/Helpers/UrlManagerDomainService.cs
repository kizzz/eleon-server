using Common.Module.Constants;
using Eleon.Logging.Lib.SystemLog.Logger;
using Logging.Module;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace VPortal.Infrastructure.Module.Domain.DomainServices
{


  public static class UrlManagerDomainService
  {

    public static async Task<string> GetDocumentUrl(string id, Guid? tenantId)
    {
      try
      {
        // TODO: artem, localhost get from tenant
        // TODO: artem, angular url from configuration
        return $"http://localhost:4200/vportal/details/{id}";
      }
      catch (Exception e)
      {
        EleonsoftLog.Error("Error while generating document url", e);
        throw;
      }
    }
  }

}
