using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Application.Contracts.Currency;
using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Currency;
using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Currency;
using VPortal.Infrastructure.Module;

namespace ModuleCollector.Infrastructure.Module.Infrastructure.Module.Application.Currency;

[Authorize]
public class CurrencyAppService : InfrastructureAppService, ICurrencyAppService
{
  private readonly IVportalLogger<CurrencyAppService> _logger;
  private readonly CurrencyDomainService _currencyDomainService;

  public CurrencyAppService(
      IVportalLogger<CurrencyAppService> logger,
      CurrencyDomainService currencyDomainService)
  {
    _logger = logger;
    _currencyDomainService = currencyDomainService;
  }
  public async Task<List<CurrencyDto>> GetCurrenciesAsync()
  {

    try
    {
      var entities = await _currencyDomainService.GetCurrenciesAsync();
      return ObjectMapper.Map<List<CurrencyEntity>, List<CurrencyDto>>(entities);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<CurrencyRateDto> GetCurrencyRateAsync(string from, string to, DateTime? rateDate)
  {

    try
    {
      var entities = await _currencyDomainService.GetCurrencyRateAsync(from, to, rateDate);
      return ObjectMapper.Map<CurrencyRateEntity, CurrencyRateDto>(entities);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<List<CurrencyRateDto>> GetCurrencyRatesAsync(string from, DateTime? rateDate)
  {

    try
    {
      var entities = await _currencyDomainService.GetCurrencyRatesAsync(from, rateDate);
      return ObjectMapper.Map<List<CurrencyRateEntity>, List<CurrencyRateDto>>(entities);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<CurrencyDto> GetSystemCurrencyAsync()
  {

    try
    {
      var currency = await _currencyDomainService.GetSystemCurrencyAsync();
      return ObjectMapper.Map<CurrencyEntity, CurrencyDto>(currency);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<bool> SetSystemCurrencyAsync(string code)
  {

    try
    {
      await _currencyDomainService.SetSystemCurrencyAsync(code);
      return true;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }

    return false;
  }
}
