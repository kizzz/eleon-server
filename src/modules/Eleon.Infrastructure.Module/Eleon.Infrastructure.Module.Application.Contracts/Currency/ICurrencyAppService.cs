using Volo.Abp.Application.Services;

namespace ModuleCollector.Infrastructure.Module.Infrastructure.Module.Application.Contracts.Currency;

public interface ICurrencyAppService : IApplicationService
{
  Task<CurrencyDto> GetSystemCurrencyAsync();
  Task<bool> SetSystemCurrencyAsync(string code);
  Task<List<CurrencyDto>> GetCurrenciesAsync();

  Task<List<CurrencyRateDto>> GetCurrencyRatesAsync(string from, DateTime? rateDate);

  Task<CurrencyRateDto> GetCurrencyRateAsync(string from, string to, DateTime? rateDate);
}
