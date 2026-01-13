namespace Common.Module.Keys
{
  public class SignInOtpKey : RawCompoundKey
  {
    private const string Prefix = "SignInOtp_";

    public SignInOtpKey(Guid userId)
        : base(Prefix, userId.ToString())
    {
    }
  }
}
