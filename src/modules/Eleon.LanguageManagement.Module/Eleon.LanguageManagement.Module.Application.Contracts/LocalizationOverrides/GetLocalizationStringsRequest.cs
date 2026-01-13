using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{
  public class GetLocalizationStringsRequest : PagedAndSortedResultRequestDto
  {
    public string SearchQuery { get; set; }
    public string TargetCulture { get; set; }
    public string BaseCulture { get; set; }
    public bool OnlyEmpty { get; set; }
    public List<string> LocalizationResources { get; set; }
  }
}
