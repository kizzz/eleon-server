using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Entities
{
  public class StorageProviderTypeEntity : Entity<Guid>
  {
    public string Name { get; set; } = string.Empty;
    public string Parent { get; set; } = string.Empty;
    //public bool Testable { get; set; } 
    protected StorageProviderTypeEntity() { }
    public StorageProviderTypeEntity(Guid id)
        : base(id)
    {
    }
  }
}
