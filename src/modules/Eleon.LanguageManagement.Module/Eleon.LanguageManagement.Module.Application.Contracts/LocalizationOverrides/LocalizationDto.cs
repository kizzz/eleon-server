using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{
  public class LocalizationResourceDto
  {
    public string ResourceName { get; set; }
    public Dictionary<string, string> Texts { get; set; }
  }
  public class LocalizationDto
  {
    public string Culture { get; set; }
    public List<LocalizationResourceDto> Resources { get; set; }
  }
}
