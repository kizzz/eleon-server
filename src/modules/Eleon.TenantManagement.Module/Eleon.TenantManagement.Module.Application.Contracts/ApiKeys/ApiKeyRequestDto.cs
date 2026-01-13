using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Identity.Module.Identity.Module.Application.Contracts.ApiKeys;
public class ApiKeyRequestDto
{
  public List<ApiKeyType> KeyTypes { get; set; } = new List<ApiKeyType>();
}
