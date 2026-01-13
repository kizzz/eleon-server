using System;
using Volo.Abp.Domain.Entities;
using VPortal.Accounting.Module.Constants;

namespace VPortal.Accounting.Module.Entities
{
  public class MemberEntity : Entity<Guid>
  {
    public Guid RefId { get; set; }
    public MemberType Type { get; set; }
  }
}

