using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.TenantManagement.Module.TenantLocalization
{
  public interface ITenantLocalizationAppService : IApplicationService
  {
    public Task<LanguageEntryDto> GetTenantLanguage();
    public Task SetTenantLanguage(string cultureName, string uiCultureName);
  }
}
