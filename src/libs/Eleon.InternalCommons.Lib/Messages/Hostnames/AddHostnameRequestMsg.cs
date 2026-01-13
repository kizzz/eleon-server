using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.InternalCommons.Lib.Messages.Hostnames;
public class AddHostnameRequestMsg
{
  public Guid? TenantId { get; set; }
  public string Domain { get; set; }
  public string TenantName { get; set; }
  public bool AcceptClientCertificate { get; set; }
  public bool IsSsl { get; set; }
  public VportalApplicationType ApplicationType { get; set; }
  public bool IsDefault { get; set; }
  public int Port { get; set; }
  public Guid? AppId { get; set; }
  public bool IsInternal { get; set; }
}
