using System;

namespace VPortal.LanguageManagement.Module.Languages
{
  public class SetLanguageEnabledDto
  {
    public Guid LanguageId { get; set; }
    public bool IsEnabled { get; set; }
  }
}
