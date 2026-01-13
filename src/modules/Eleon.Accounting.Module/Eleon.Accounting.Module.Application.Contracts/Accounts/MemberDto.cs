using System;
using System.Collections.Generic;
using System.Text;
using VPortal.Accounting.Module.Constants;

namespace VPortal.Accounting.Module.Accounts
{
  public class MemberDto
  {
    public Guid Id { get; set; }
    public Guid RefId { get; set; }
    public MemberType Type { get; set; }
  }
}
