using Common.EventBus.Module;
using Eleon.InternalCommons.Lib.Messages.Hostnames;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module.DomainServices;

namespace Eleon.TenantManagement.Module.Eleon.TenantManagement.Module.Application.EventServices;
public class AddHostnameEventHandler : IDistributedEventHandler<AddHostnameRequestMsg>, ITransientDependency
{
  private readonly IResponseContext _responseContext;
  private readonly IVportalLogger<AddHostnameEventHandler> _logger;
  private readonly TenantHostnameDomainService _tenantHostnameDomainService;

  public AddHostnameEventHandler(
    IResponseContext responseContext,
    IVportalLogger<AddHostnameEventHandler> logger,
    TenantHostnameDomainService tenantHostnameDomainService)
  {
    _responseContext = responseContext;
    _logger = logger;
    _tenantHostnameDomainService = tenantHostnameDomainService;
  }

  public async Task HandleEventAsync(AddHostnameRequestMsg eventData)
  {
    var response = new AddHostnameResponseMsg
    {
      Success = false,
      Message = "Failed to add hostname",
      HostnameId = Guid.Empty
    };


    try
    {
      var result = await _tenantHostnameDomainService.AddNonInternalTenantHostname(
        eventData.TenantId,
        eventData.Domain,
        eventData.TenantName,
        eventData.AcceptClientCertificate,
        eventData.IsSsl,
        eventData.ApplicationType,
        eventData.IsDefault,
        eventData.Port,
        eventData.AppId,
        eventData.IsInternal
        );
      response.Success = true;
      response.Message = "Hostname added successfully";
      response.HostnameId = result.Id;
    }
    catch (Exception ex)
    {
      response.Success = false;
      response.Message = $"Error adding hostname: {ex.Message}";
      _logger.Capture(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
