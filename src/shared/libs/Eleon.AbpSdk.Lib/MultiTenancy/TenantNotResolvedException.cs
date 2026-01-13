using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.MultiTenancy;

public class TenantNotResolvedException : Exception
{
  public TenantNotResolvedException()
  {

  }

  public TenantNotResolvedException(string message) : base(message)
  {

  }

  public TenantNotResolvedException(string message, Exception innerException) : base(message, innerException)
  {

  }
}