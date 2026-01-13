using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Storage.Module.Storage.Module.Application.Contracts.StorageProviders;
public class StorageProviderSaveResponseDto
{
  public Guid? Id { get; set; }
  public bool IsTested { get; set; }
}
