using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Infrastructure.Module.Infrastructure.Module.Application.Contracts.Currency;
public class CurrencyRateDto
{
  public string FromCurrencyCode { get; set; }
  public string ToCurrencyCode { get; set; }
  public decimal Rate { get; set; }
  public DateTime RateDate { get; set; }
}
