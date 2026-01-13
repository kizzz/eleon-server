using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Messages.Hostnames;
public class HostnameEto
{
  public Guid? AppId { get; set; }
  public string Url { get; set; }
}
