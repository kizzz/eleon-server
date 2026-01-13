using Microsoft.Extensions.Configuration;

namespace VPortal.Otp.Module.Options
{
  public class OtpOptions
  {
    public bool UseStub { get; set; }

    public OtpOptions PreConfigure(IConfiguration configuration)
    {
      var opt = configuration
          .GetSection("Otp")
          .Get<OtpOptions>();

      if (opt != null)
      {
        UseStub = opt.UseStub;
      }

      return this;
    }
  }
}
