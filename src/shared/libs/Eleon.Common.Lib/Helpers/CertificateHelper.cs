using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Common.Module.Helpers
{
  public class CertificateHelper
  {
    public static bool ValidateCertificateHash(X509Certificate2 cert, string targetHash)
    {
      var certHash = GetCertificateHash(cert);
      return certHash.Equals(targetHash, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetCertificateHash(X509Certificate2 cert)
    {
      string certStr = cert.GetRawCertDataString();
      var inputHash = SHA256.HashData(Encoding.UTF8.GetBytes(certStr));
      return Convert.ToHexString(inputHash);
    }

    public static X509Certificate2 CreateCertificateFromBase64(string certBase64, string password)
    {
      var bytes = Convert.FromBase64String(certBase64);
      return CreateCertificateFromPem(bytes, password);
    }

    public static void SaveCertificateToLocalMachineRoot(X509Certificate2 cert)
    {
      X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
      store.Open(OpenFlags.ReadWrite);
      if (!string.IsNullOrWhiteSpace(cert.FriendlyName))
      {
        var existing = store.Certificates.FirstOrDefault(x => x.FriendlyName == cert.FriendlyName);
        if (existing != null)
        {
          store.Remove(existing);
        }
      }

      store.Add(cert);
      store.Close();
    }

    public static async Task<X509Certificate2> LoadCertificateFromFile(string filepath, string password)
    {
      var bytes = await File.ReadAllBytesAsync(filepath);
      return CreateCertificateFromPem(bytes, password);
    }

    public static X509Certificate2 CreateCertificateFromPem(byte[] pem, string password)
    {
      var effectivePassword = string.IsNullOrWhiteSpace(password) ? null : password;
      return X509CertificateLoader.LoadPkcs12(pem, effectivePassword);
    }

    public static X509Certificate2 GenerateSelfSignedCertificate(string hostname, string friendlyName, string password)
    {
      RSA rsa = RSA.Create();

      var request = new CertificateRequest(
          $"CN={hostname}",
          rsa,
          HashAlgorithmName.SHA256,
          RSASignaturePadding.Pkcs1);

      X509Certificate2 certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(10));
      if (OperatingSystem.IsWindows())
      {
        certificate.FriendlyName = friendlyName;
      }

      var pfxBytes = certificate.Export(X509ContentType.Pfx, password);
      return X509CertificateLoader.LoadPkcs12(
          pfxBytes,
          password,
          X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
    }

    public static byte[] GetCertificateBytes(X509Certificate2 certificate, string password)
    {
      byte[] certBytes = string.IsNullOrEmpty(password)
          ? certificate.Export(X509ContentType.Pfx)
          : certificate.Export(X509ContentType.Pfx, password);

      return certBytes;
    }

    public static string GetCertificateBase64(X509Certificate2 certificate, string password)
    {
      var bytes = GetCertificateBytes(certificate, password);
      return Convert.ToBase64String(bytes);
    }
  }
}
