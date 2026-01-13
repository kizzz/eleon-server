using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.LanguageManagement.Module.Languages
{
  public interface ILanguageAppService : IApplicationService
  {
    Task<List<LanguageDto>> GetLanguageList();
    Task<bool> SetLanguageEnabled(SetLanguageEnabledDto request);
    Task<bool> SetDefaultLanguage(Guid languageId);
    Task<LanguageInfoDto> GetDefaultLanguage();
  }
}
