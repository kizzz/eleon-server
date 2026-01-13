using Infrastructure.Module.Localization;
using Microsoft.Extensions.Localization;
using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Constants;
using Volo.Abp.Settings;

namespace VPortal.Infrastructure.Module.Settings;

public class ModuleSettingDefinitionProvider : SettingDefinitionProvider
{
  public override void Define(ISettingDefinitionContext context)
  {
    context.Add(new SettingDefinition(CurrencyConstants.DefaultCurrencySetting, "USD"));
  }
}
