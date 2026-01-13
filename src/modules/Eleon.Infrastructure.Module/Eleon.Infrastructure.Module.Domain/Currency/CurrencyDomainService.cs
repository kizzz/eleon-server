using Logging.Module;
using Microsoft.Extensions.Logging;
using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Constants;
using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Currency;
using System.Text.Json;
using Volo.Abp.Domain.Services;
using Volo.Abp.SettingManagement;

namespace ModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Currency;
public class CurrencyDomainService : DomainService
{
  private readonly IVportalLogger<CurrencyDomainService> _logger;
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly ISettingManager _settingManager;

  public CurrencyDomainService(
      IVportalLogger<CurrencyDomainService> logger,
      IHttpClientFactory httpClientFactory,
      ISettingManager settingManager)
  {
    _logger = logger;
    _httpClientFactory = httpClientFactory;
    _settingManager = settingManager;
  }

  public async Task<CurrencyEntity> GetSystemCurrencyAsync()
  {
    try
    {
      var currency = await _settingManager.GetOrNullForCurrentTenantAsync(CurrencyConstants.DefaultCurrencySetting, true);
      return CurrencyConstants.Currencies.FirstOrDefault(c => c.Code == currency) ?? CurrencyConstants.DefaultCurrency;
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

  public async Task SetSystemCurrencyAsync(string code)
  {
    try
    {
      var insertCode = CurrencyConstants.Currencies.FirstOrDefault(c => c.Code == code)?.Code ?? CurrencyConstants.DefaultCurrency.Code;
      await _settingManager.SetForCurrentTenantAsync(CurrencyConstants.DefaultCurrencySetting, insertCode, true);
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

  public async Task<List<CurrencyEntity>> GetCurrenciesAsync()
  {

    try
    {
      return CurrencyConstants.Currencies;
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

  public async Task<List<CurrencyRateEntity>> GetCurrencyRatesAsync(string from, DateTime? rateDate)
  {

    try
    {
      using var client = _httpClientFactory.CreateClient();

      var url = CurrencyConstants.GetUrl(from, rateDate: rateDate);

      var response = await client.GetAsync(url);

      if (!response.IsSuccessStatusCode)
      {
        _logger.Log.LogError($"Failed to fetch currency rates. Status code: {response.StatusCode} Reason: {response.ReasonPhrase}");
        throw new Exception($"Failed to fetch currency rates. Status code: {response.StatusCode}  Reason: {response.ReasonPhrase}");
      }

      var content = await response.Content.ReadAsStringAsync();

      return DeserializeCurrencyRates(content);
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

  public async Task<CurrencyRateEntity> GetCurrencyRateAsync(string from, string to, DateTime? rateDate)
  {

    try
    {
      rateDate ??= DateTime.UtcNow;

      using var client = _httpClientFactory.CreateClient();

      var url = CurrencyConstants.GetUrl(from, to, rateDate: rateDate);

      var response = await client.GetAsync(url);

      if (!response.IsSuccessStatusCode)
      {
        _logger.Log.LogError($"Failed to fetch currency rate. Status code: {response.StatusCode} Reason: {response.ReasonPhrase}");
        throw new Exception($"Failed to fetch currency rate. Status code: {response.StatusCode}  Reason: {response.ReasonPhrase}");
      }

      var content = await response.Content.ReadAsStringAsync();
      return DeserializeCurrencyRates(content).FirstOrDefault();
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
  private static List<CurrencyRateEntity> DeserializeCurrencyRates(string json)
  {
    using var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;

    var fromCurrency = root.GetProperty("base").GetString();
    var rateDate = root.GetProperty("date").GetDateTime();

    var rates = root.GetProperty("rates");
    var result = new List<CurrencyRateEntity>();

    foreach (var rate in rates.EnumerateObject())
    {
      result.Add(new CurrencyRateEntity
      {
        FromCurrencyCode = fromCurrency,
        ToCurrencyCode = rate.Name,
        Rate = rate.Value.GetDecimal(),
        RateDate = rateDate
      });
    }

    return result;
  }
}
