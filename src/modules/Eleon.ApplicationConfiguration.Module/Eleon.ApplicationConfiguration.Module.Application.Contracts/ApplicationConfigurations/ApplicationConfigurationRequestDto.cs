using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.EleoncoreApplicationConfiguration;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.AspNetCore.Mvc.MultiTenancy;
using VPortal.SitesManagement.Module.ClientApplications;
using VPortal.SitesManagement.Module.Microservices;

namespace Eleon.ApplicationConfiguration.Module.ApplicationConfigurations.Dtos;

public class ApplicationConfigurationRequestDto
{
  public string ApplicationIdentifier { get; set; }
}
