using ModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Currency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Shared.Constants;
public static class CurrencyConstants
{
  public const string DefaultCurrencySetting = "DefaultCurrencySetting";
  public static CurrencyEntity DefaultCurrency => new CurrencyEntity
  {
    Code = "USD",
    Symbol = "$"
  };
  public static List<CurrencyEntity> Currencies => new List<CurrencyEntity>()
    {
        new () { Code = "USD", Symbol = "$" },
        new () { Code = "AUD", Symbol = "A$" },
        new () { Code = "BGN", Symbol = "лв" },
        new () { Code = "BRL", Symbol = "R$" },
        new () { Code = "CAD", Symbol = "C$" },
        new () { Code = "CHF", Symbol = "CHF" },
        new () { Code = "CNY", Symbol = "¥" },
        new () { Code = "CZK", Symbol = "Kč" },
        new () { Code = "DKK", Symbol = "kr" },
        new () { Code = "EUR", Symbol = "€" },
        new () { Code = "GBP", Symbol = "£" },
        new () { Code = "HKD", Symbol = "HK$" },
        new () { Code = "HUF", Symbol = "Ft" },
        new () { Code = "IDR", Symbol = "Rp" },
        new () { Code = "ILS", Symbol = "₪" },
        new () { Code = "INR", Symbol = "₹" },
        new () { Code = "ISK", Symbol = "kr" },
        new () { Code = "JPY", Symbol = "¥" },
        new () { Code = "KRW", Symbol = "₩" },
        new () { Code = "MXN", Symbol = "Mex$" },
        new () { Code = "MYR", Symbol = "RM" },
        new () { Code = "NOK", Symbol = "kr" },
        new () { Code = "NZD", Symbol = "NZ$" },
        new () { Code = "PHP", Symbol = "₱" },
        new () { Code = "PLN", Symbol = "zł" },
        new () { Code = "RON", Symbol = "lei" },
        new () { Code = "SEK", Symbol = "kr" },
        new () { Code = "SGD", Symbol = "S$" },
        new () { Code = "THB", Symbol = "฿" },
        new () { Code = "TRY", Symbol = "₺" },
        new () { Code = "ZAR", Symbol = "R" }
    };

  public const string CurrencyDataProviderUrl = "https://api.frankfurter.app/";

  public static string GetUrl(string from, string to = "", DateTime? rateDate = null)
  {
    ArgumentNullException.ThrowIfNullOrWhiteSpace(from, nameof(from));

    rateDate ??= DateTime.UtcNow;
    var datePart = rateDate.Value.ToString("yyyy-MM-dd");
    var url = $"{CurrencyDataProviderUrl}{datePart}?from={from}";
    if (!string.IsNullOrWhiteSpace(to))
    {
      url += $"&to={to}";
    }
    return url;
  }
}
