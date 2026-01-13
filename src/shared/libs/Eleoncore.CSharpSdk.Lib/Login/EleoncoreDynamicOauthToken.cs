namespace Eleoncore.SDK.Login
{
  //internal class EleoncoreDynamicOauthToken : OAuthToken
  //{
  //    private readonly EleoncoreCurrentAuthParams authParams;

  //    public EleoncoreDynamicOauthToken(EleoncoreCurrentAuthParams authParams, AuthManger authManger, TimeSpan? timeout = null) : base(null, timeout)
  //    {
  //        this.authParams = authParams;
  //    }

  //    public override void UseInHeader(HttpRequestMessage request, string headerName)
  //    {
  //        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetToken());
  //    }

  //    private string GetToken()
  //    {
  //        return authParams.AccessToken;
  //    }
  //}
}
