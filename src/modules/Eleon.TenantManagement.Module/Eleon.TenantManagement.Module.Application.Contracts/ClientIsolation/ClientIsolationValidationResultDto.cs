using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.TenantManagement.Module.ClientIsolation
{
  public class ClientIsolationValidationResultDto
  {

    public ClientIsolationValidationResult ValidationResult { get; }
    public bool Valid { get; set; }
    public string Ip { get; set; }
    public string ValidatedCertificateName { get; set; }
    public string RedirectUrl { get; set; }
  }
}
