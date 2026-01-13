using EleonsoftSdk.Messages.Permissions;
using Logging.Module;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.PermissionManagement;
using VPortal.TenantManagement.Module.DomainServices;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.EventServices;
public class SendPermissionsEventHandler : IDistributedEventHandler<SendPermissionsMsg>, ITransientDependency
{
  private readonly IVportalLogger<SendPermissionsEventHandler> _logger;
  private readonly CustomPermissionDomainService _customPermissionDomainService;
  private readonly IGuidGenerator _guidGenerator;

  public SendPermissionsEventHandler(
      IVportalLogger<SendPermissionsEventHandler> logger,
      CustomPermissionDomainService customPermissionDomainService,
      IGuidGenerator guidGenerator)
  {
    _logger = logger;
    _customPermissionDomainService = customPermissionDomainService;
    _guidGenerator = guidGenerator;
  }
  public async Task HandleEventAsync(SendPermissionsMsg eventData)
  {
    try
    {
      var groups = eventData.Groups.Select(x =>
      {
        var record = new PermissionGroupDefinitionRecord(_guidGenerator.Create(), x.Name, x.DisplayName);
        x.ExtraProperties.ForEach(prop =>
              {
            record.SetProperty(prop.Key, prop.Value);
          });
        return record;
      }).ToList();

      var permissions = eventData.Permissions.Select(x =>
      {
        var record = new PermissionDefinitionRecord(_guidGenerator.Create(), x.GroupName, x.Name, x.ParentName, x.DisplayName, x.IsEnabled, x.MultiTenancySide, x.Providers, x.StateCheckers);
        x.ExtraProperties.ForEach(prop =>
              {
            record.SetProperty(prop.Key, prop.Value);
          });
        return record;
      }).ToList();

      await _customPermissionDomainService.CreateGroupsForMicroserviceAsync(string.Empty, groups);
      await _customPermissionDomainService.CreatePermissionsForMicroserviceAsync(string.Empty, permissions);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
    finally
    {
    }
  }
}
