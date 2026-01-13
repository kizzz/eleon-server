using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Storage.Lib.Models
{
  public class StorageProviderTypeDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Parent { get; set; } = string.Empty;
  }
}
