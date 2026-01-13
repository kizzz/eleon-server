using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{
  public class GetLocalizationRequest
  {
    public string Culture { get; set; }
    public List<string> LocalizationResources { get; set; }
  }
}
