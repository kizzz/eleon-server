using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Module.ClientIsolation
{
  public enum ClientIsolationValidationResult
  {
    MissingClientCert = 0,
    InvalidTenantCert = 1,
    InvalidUserCert = 2,
    ValidTenantCert = 3,
    ValidUserCert = 4,
    NothingToValidate = 5,
    ValidIp = 6,
    InvalidIp = 7,
    CertIsolationDisabled = 8,
  }

  public class ClientIsolationContextValidationResult
  {
    public ClientIsolationValidationResult ValidationResult { get; }
    public bool Valid { get; }
    public string Ip { get; set; }
    public string ValidatedCertificateName { get; set; }

    public ClientIsolationContextValidationResult(ClientIsolationValidationResult validationResult)
    {
      ValidationResult = validationResult;
      Valid = IsValid(validationResult);
    }

    internal static bool IsValid(ClientIsolationValidationResult validationResult)
        => validationResult
            is ClientIsolationValidationResult.ValidTenantCert
            or ClientIsolationValidationResult.ValidUserCert
            or ClientIsolationValidationResult.ValidIp
            or ClientIsolationValidationResult.NothingToValidate
            or ClientIsolationValidationResult.CertIsolationDisabled;
  }
}
