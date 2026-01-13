using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Application.Contracts.Currency;
using Volo.Abp;
using VPortal.Infrastructure.Module;

namespace ModuleCollector.Infrastructure.Module.Infrastructure.Module.HttpApi.Currency;

[Area(InfrastructureRemoteServiceConsts.ModuleName)]
[RemoteService(Name = InfrastructureRemoteServiceConsts.RemoteServiceName)]
[Route("api/infrastructure/currency")]
public class CurrencyController : InfrastructureController, ICurrencyAppService
{
  private readonly IVportalLogger<CurrencyController> _logger;
  private readonly ICurrencyAppService _currencyAppService;

  public CurrencyController(
      IVportalLogger<CurrencyController> logger,
      ICurrencyAppService currencyAppService)
  {
    _logger = logger;
    _currencyAppService = currencyAppService;
  }

  [HttpGet("GetCurrencies")]
  public Task<List<CurrencyDto>> GetCurrenciesAsync()
  {
    try
    {
      return _currencyAppService.GetCurrenciesAsync();
    }
    finally
    {
    }
  }

  [HttpGet("GetCurrencyRate")]
  public async Task<CurrencyRateDto> GetCurrencyRateAsync(string from, string to, DateTime? rateDate)
  {
    try
    {
      return await _currencyAppService.GetCurrencyRateAsync(from, to, rateDate);
    }
    finally
    {
    }
  }

  [HttpGet("GetCurrencyRates")]
  public async Task<List<CurrencyRateDto>> GetCurrencyRatesAsync(string from, DateTime? rateDate)
  {
    try
    {
      return await _currencyAppService.GetCurrencyRatesAsync(from, rateDate);
    }
    finally
    {
    }
  }

  [HttpGet("GetSystemCurrency")]
  public async Task<CurrencyDto> GetSystemCurrencyAsync()
  {
    try
    {
      return await _currencyAppService.GetSystemCurrencyAsync();
    }
    finally
    {
    }
  }

  [HttpPost("SetSystemCurrency")]
  public async Task<bool> SetSystemCurrencyAsync(string code)
  {
    try
    {
      return await _currencyAppService.SetSystemCurrencyAsync(code);
    }
    finally
    {
    }
  }
}
