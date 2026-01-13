using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.TenantManagement.Module.Microservices
{
  public class GetServiceConnectionStringEventService :
      IDistributedEventHandler<RequestMicroserviceConnectionStringMsg>,
      ITransientDependency
  {
    private readonly IConnectionStringResolver connectionStringResolver;
    private readonly IResponseContext responseContext;
    private readonly IVportalLogger<GetServiceConnectionStringEventService> logger;

    public GetServiceConnectionStringEventService(
        IConnectionStringResolver connectionStringResolver,
        IResponseContext responseContext,
        IVportalLogger<GetServiceConnectionStringEventService> logger)
    {
      this.connectionStringResolver = connectionStringResolver;
      this.responseContext = responseContext;
      this.logger = logger;
    }

    public async Task HandleEventAsync(RequestMicroserviceConnectionStringMsg eventData)
    {
      var response = new SetMicroserviceConnectionStringMsg();
      try
      {
        string str = await connectionStringResolver.ResolveAsync();
        response.ConnectionString = str;
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
