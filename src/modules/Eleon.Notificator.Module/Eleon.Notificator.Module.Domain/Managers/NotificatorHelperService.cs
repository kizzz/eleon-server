using Common.Module.Constants;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TextTemplating;
using VPortal.Notificator.Module.Localization;
using VPortal.Notificator.Module.Notificators;
using VPortal.Notificator.Module.Notificators.Implementations;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;

public enum NotificatorAddressType
{
  Email,
  Phone,
  Id
}

public class NotificatorHelperService : NotificatorBaseHelperService, ITransientDependency
{
  private readonly IdentityRoleManager roleManager;
  private readonly IdentityUserManager userManager;
  private readonly IOrganizationUnitRepository _organizationUnitRepository;
  private readonly ITemplateRenderer _templateRenderer;

  public NotificatorHelperService(
      IdentityRoleManager roleManager,
      IdentityUserManager userManager,
      IOrganizationUnitRepository organizationUnitRepository,
      ITemplateRenderer templateRenderer,
      IServiceProvider serviceProvider) : base(serviceProvider)
  {
    this.roleManager = roleManager;
    this.userManager = userManager;
    _organizationUnitRepository = organizationUnitRepository;
    _templateRenderer = templateRenderer;
  }

  public async Task<string> GetAddressAsync(NotificatorRecepientType type, string refId, NotificatorAddressType addressType, string defaultAddress)
  {
    if (type == NotificatorRecepientType.Direct || string.IsNullOrWhiteSpace(refId))
    {
      return defaultAddress;
    }

    switch (addressType)
    {
      case NotificatorAddressType.Email:
        return (await userManager.FindByIdAsync(refId))?.Email ?? defaultAddress;
      case NotificatorAddressType.Phone:
        return (await userManager.FindByIdAsync(refId))?.PhoneNumber ?? defaultAddress;
      case NotificatorAddressType.Id:
        return (await userManager.FindByIdAsync(refId))?.Id.ToString() ?? defaultAddress;
      default:
        return defaultAddress;
    }
  }

  public async Task<List<Guid>> ResolveRecepientIdsAsync(NotificatorRecepientType recepientType, string recepientRefId)
  {
    var recepients = new List<Guid>();
    switch (recepientType)
    {
      case NotificatorRecepientType.Direct:
      case NotificatorRecepientType.User:
        if (Guid.TryParse(recepientRefId, out var directId))
        {
          recepients.Add(directId);
        }
        break;
      case NotificatorRecepientType.Role:
        var role = await roleManager.FindByIdAsync(recepientRefId);
        if (role != null)
        {
          var users = await userManager.GetUsersInRoleAsync(role.Name);
          recepients.AddRange(users.Select(x => x.Id));
        }
        break;
      case NotificatorRecepientType.OrganizationUnit:
        if (Guid.TryParse(recepientRefId, out var orgUnitId))
        {
          var orgUnit = await _organizationUnitRepository.FindAsync(orgUnitId);

          if (orgUnit != null)
          {
            var users = await userManager.GetUsersInOrganizationUnitAsync(orgUnit);
            recepients.AddRange(users.Select(x => x.Id));
          }
        }
        break;
      default:
        throw new NotImplementedException();
    }

    return recepients;
  }

  public async Task<string> RenderTemplateAsync(string templateName, string[] data)
  {
    return await _templateRenderer.RenderAsync(templateName, data);
  }
}
