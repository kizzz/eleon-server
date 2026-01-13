using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;

namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{
  public interface ILocalizationOverrideAppService : IApplicationService
  {
    Task<LocalizationDto> GetLocalization(GetLocalizationRequest getLocalizationRequest);
    Task<bool> OverrideLocalizationEntry(OverrideLocalizationEntryRequest request);
    Task<PagedResultDto<OverriddenLocalizationStringDto>> GetLocalizationStrings(GetLocalizationStringsRequest request);
    Task<LocalizationInformationDto> GetLocalizationInformation();
  }
}
