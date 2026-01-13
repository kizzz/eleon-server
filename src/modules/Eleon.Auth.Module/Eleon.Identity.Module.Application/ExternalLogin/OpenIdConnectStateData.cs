using System;
using System.Security.Cryptography;
using System.Text;

namespace ExternalLogin.Module
{
  internal class OpenIdConnectStateData
  {
    public string Salt { get; set; }
    public DateTime Date { get; set; }
    public Guid? TenantId { get; set; }
    public string AuthScheme { get; set; }
    public string Signature { get; set; }

    public OpenIdConnectStateData(string salt, Guid? tenantId, string authScheme, string privateKey)
    {
      Salt = salt;
      TenantId = tenantId;
      AuthScheme = authScheme;
      Date = DateTime.UtcNow;
      Signature = GetHashSignature(privateKey);
    }

    public OpenIdConnectStateData()
    {
    }

    public void Validate(string salt, string privateKey)
    {
      if (Salt.IsNullOrWhiteSpace() || Date == default)
      {
        throw new Exception("OIDC message is corrupted.");
      }

      if (Salt != salt || DateTime.UtcNow.Subtract(Date).TotalMinutes >= 10)
      {
        throw new Exception("OIDC message is expired.");
      }

      ValidateSignature(privateKey);
    }

    private void ValidateSignature(string privateKey)
    {
      string actualSignature = GetHashSignature(privateKey);
      bool signatureValid = actualSignature == Signature;
      if (!signatureValid)
      {
        throw new Exception("OIDC message is corrupted.");
      }
    }

    private string GetHashSignature(string privateKey)
    {
      string inputString = $"{Salt}{Date}{TenantId}{AuthScheme}";
      string dataHash = GenerateSha256Hash(inputString);
      string signature = GenerateSha256Hash(inputString + dataHash + privateKey);
      return signature;
    }

    private static string GenerateSha256Hash(string input)
    {
      var inputBytes = Encoding.UTF8.GetBytes(input);
      var inputHash = SHA256.HashData(inputBytes);
      return Convert.ToHexString(inputHash);
    }
  }
}
