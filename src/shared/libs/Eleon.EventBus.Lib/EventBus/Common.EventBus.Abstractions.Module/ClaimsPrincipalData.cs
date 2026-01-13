namespace Common.Module.Serialization
{
  public class ClaimsPrincipalData
  {
    public List<ClaimsIdentityData> Identities { get; set; }

    public class ClaimsIdentityData
    {
      public ClaimsIdentityData()
      {
        Claims = new List<ClaimData>();
      }

      public List<ClaimData> Claims { get; set; }
      public string AuthenticationType { get; set; }

      public class ClaimData
      {
        public ClaimData()
        {
          Type = string.Empty;
          Value = string.Empty;
        }

        public required string Type { get; set; }
        public required string Value { get; set; }
      }
    }

    public System.Security.Claims.ClaimsPrincipal ToClaimsPrincipal()
    {
      var claimsIdentities = new List<System.Security.Claims.ClaimsIdentity>();
      foreach (var identity in Identities)
      {
        var claims = new List<System.Security.Claims.Claim>();
        foreach (var claim in identity.Claims)
        {
          claims.Add(new System.Security.Claims.Claim(claim.Type, claim.Value));
        }

        var identityData = new System.Security.Claims.ClaimsIdentity(claims, authenticationType: identity.AuthenticationType);
        claimsIdentities.Add(identityData);
      }

      return new System.Security.Claims.ClaimsPrincipal(claimsIdentities);
    }

    public ClaimsPrincipalData()
    {
    }

    public ClaimsPrincipalData(System.Security.Claims.ClaimsPrincipal claimsPrincipal)
    {
      Identities = new List<ClaimsIdentityData>();
      if (claimsPrincipal != null)
      {
        foreach (var identity in claimsPrincipal.Identities)
        {
          var identityData = new ClaimsIdentityData();
          identityData.Claims = new List<ClaimsIdentityData.ClaimData>();
          foreach (var claim in identity.Claims)
          {
            identityData.Claims.Add(new ClaimsIdentityData.ClaimData
            {
              Type = claim.Type,
              Value = claim.Value,
            });
          }
          identityData.AuthenticationType = identity.AuthenticationType;

          Identities.Add(identityData);
        }
      }
    }
  }
}
