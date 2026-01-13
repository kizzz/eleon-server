using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.AppConfiguration;
public interface IApplicationConfigurationAppService : IApplicationService
{
  Task<ApplicationConfigurationDto> GetAsync(AppConfigRequestDto request);
}
