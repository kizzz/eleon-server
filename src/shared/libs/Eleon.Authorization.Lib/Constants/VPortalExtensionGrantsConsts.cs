namespace Common.Module.Constants
{
  public class VPortalExtensionGrantsConsts
  {
    public class Names
    {
      public const string MachineKeyGrant = "x_machine_key";
      public const string ApiKeyGrant = "x_api_key";
      public const string ImpresonationGrant = "Impersonation";
    }

    public class Impresonation
    {
      public const string AccessTokenParameter = "access_token";
      public const string ImpersonatedUserParameter = "impersonated_user";
      public const string ImpersonatedTenantParameter = "impersonated_tenant";
      public const string ImpersonatorUserClaim = "imp_user";
      public const string ImpersonatorTenantClaim = "imp_tenant";
    }

    public class MachineKey
    {
      public const string MachineKeyParameter = "machine_key";
      public const string MachineKeyHeader = "X-Machine-Key";
      public const string MachineKeyClaim = "machine_key";
    }

    public class ApiKey
    {
      public const string ApiKeyParameter = "api_key";
      public const string NonceParameter = "api_nonce";
      public const string SignatureParameter = "api_signature";
      public const string TimestampParameter = "api_timestamp";

      public const string ApiKeyId = "key_id";
      public const string ApiKeyRefIdClaim = "key_sub";
      public const string ApiKeyTypeClaim = "key_type";
      public const string ApiKeyNameClaim = "key_name";
    }
  }
}
