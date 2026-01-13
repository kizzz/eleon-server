using IdentityModel.Client;

namespace Eleoncore.SDK.Helpers
{
  public class EleoncoreConnectionHelper
  {
    public async static Task<string> GetAccessToken(string authUrl, string clientId, string clientSecret)
    {
      var client = new HttpClient();
      var discoveryDocument = await client.GetDiscoveryDocumentAsync(authUrl);

      if (discoveryDocument.IsError)
        throw new Exception(discoveryDocument.Error);

      var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
      {
        Address = discoveryDocument.TokenEndpoint,
        ClientId = clientId,
        ClientSecret = clientSecret,
      });

      if (tokenResponse.IsError)
        throw new Exception(tokenResponse.Error);

      return tokenResponse.AccessToken;
    }

    public async static Task<string> ConnectAndGetToken(string appKey, string secretKey)
    {
      throw new NotImplementedException();
    }
  }
}
